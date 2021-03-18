using ReactiveUI;
using System.Reactive;
using Core;
using System.Windows.Media.Imaging;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.IO;
using AutoWordsearch.DialogUtil;

namespace AutoWordsearch
{
    public class MainWindowViewModel : ReactiveObject
    { 
        public SaveFileDialogCallback SaveImageDialog { get; set; }
        public SaveFileDialogCallback SaveFileDialog { get; set; }
        public OpenFileDialogCallback OpenFileDialog { get; set; }
        public MessageBoxCallback ShowMessageBox { get; set; }

        public MainWindowViewModel()
        {
            WordsearchInstance = new Wordsearch();


            ExportImage = ReactiveCommand.Create(() =>
            {
                if (SaveImageDialog(WordsearchTitle, out string chosenFilePath))
                {
                    using var bitmap = WordsearchInstance.ToBitmap(WordsearchRenderOption.FillEmptyCellsWithRandomLetters);
                    bitmap.Save(chosenFilePath);
                }
            });


            ImportFile = ReactiveCommand.Create(() =>
            {
                if (OpenFileDialog(out string chosenFilePath))
                {
                    // deserialize wordsearch from file
                    var xs = new XmlSerializer(typeof(Wordsearch));
                    using var file = File.OpenRead(chosenFilePath);
                    Wordsearch ws = (Wordsearch)xs.Deserialize(file);
                    WordsearchInstance = ws;

                    // Update interface
                    Words = string.Join(Environment.NewLine, ws.Words.Select(i => i.Text));
                    this.RaisePropertyChanged(nameof(WordsearchTitle));
                    UpdatePreview();
                }
            });


            ExportFile = ReactiveCommand.Create(() =>
            {
                if (SaveFileDialog(WordsearchTitle, out string chosenFilePath))
                {
                    // serialize wordsearch to file
                    var xs = new XmlSerializer(typeof(Wordsearch));
                    using var file = File.Create(chosenFilePath);
                    xs.Serialize(file, WordsearchInstance);
                }
            });


            GenerateRandom = ReactiveCommand.Create(() =>
            {
                if (Wordsearch.TryGenerateRandom(Words.Split(Environment.NewLine), out Wordsearch ws))
                {
                    WordsearchInstance.Words = ws.Words;
                    UpdatePreview();
                }
                else
                {
                    ShowMessageBox
                    (
                        "We're having trouble fitting all of the words onto the wordsearch. Try reducing the length and number of words.",
                        "Unable to Generate Wordsearch",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error
                    );

                }
            });

            UpdatePreview();

        }

        public void UpdatePreview()
        {
            using var bmp = WordsearchInstance.ToBitmap(WordsearchRenderOption.VisibleGrid);
            PreviewSource = Conversions.BitmapToImageSource(bmp);
        }

        public ReactiveCommand<Unit, Unit> ExportImage { get; }
        public ReactiveCommand<Unit, Unit> ImportFile { get; }
        public ReactiveCommand<Unit, Unit> ExportFile { get; }
        public ReactiveCommand<Unit, Unit> GenerateRandom { get; }

        public Wordsearch WordsearchInstance { get; set; }

        public string WordsearchTitle
        {
            get => WordsearchInstance.Title;
            set
            {
                if (WordsearchInstance.Title != value)
                {
                    WordsearchInstance.Title = value;
                    this.RaisePropertyChanged();
                    UpdatePreview();
                }
            }
        }

        public string _words;
        public string Words
        {
            get => _words;
            set => this.RaiseAndSetIfChanged(ref _words, value);
        }

        private BitmapImage _previewSource;
        public BitmapImage PreviewSource
        {
            get => _previewSource;
            set => this.RaiseAndSetIfChanged(ref _previewSource, value);
        }
    }
}
