
using System.Windows.Media.Imaging;
using System.Linq;
using System;
using AutoWordsearch.DialogUtil;
using System.Windows.Input;
using System.Xml.Linq;
using AutoWordsearch.Core;

namespace AutoWordsearch
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string _words;
        private BitmapImage _previewSource;
        private Wordsearch _wordsearchInstance;

        private readonly IDialogService _dialog;
        public MainWindowViewModel(IDialogService dialog)
        {
            _dialog = dialog;
            _wordsearchInstance = new Wordsearch();

            ExportImageCommand = new RelayCommand(ExportImage);
            ImportFileCommand = new RelayCommand(ImportFile);
            ExportFileCommand = new RelayCommand(ExportFile);
            GenerateRandomCommand = new RelayCommand(GenerateRandom);

            UpdatePreview();
        }

        public ICommand ExportImageCommand { get; }
        public ICommand ImportFileCommand { get; }
        public ICommand ExportFileCommand { get; }
        public ICommand GenerateRandomCommand { get; }

        public string WordsearchTitle
        {
            get => _wordsearchInstance.Title;
            set
            {
                if (_wordsearchInstance.Title != value)
                {
                    _wordsearchInstance.Title = value;
                    RaisePropertyChanged();
                    UpdatePreview();
                }
            }
        }

        public string Words
        {
            get => _words;
            set => RaiseAndSetIfChanged(ref _words, value);
        }

        public BitmapImage PreviewSource
        {
            get => _previewSource;
            set => RaiseAndSetIfChanged(ref _previewSource, value);
        }

        private void UpdatePreview()
        {
            using (var bmp = _wordsearchInstance.ToBitmap(WordsearchRenderOption.VisibleGrid))
            {
                PreviewSource = Conversions.BitmapToImageSource(bmp);
            }
        }

        private void ExportImage()
        {
            if (_dialog.SaveImageDialog(WordsearchTitle, out string chosenFilePath))
            {
                using (var bitmap = _wordsearchInstance.ToBitmap(WordsearchRenderOption.FillEmptyCellsWithRandomLetters))
                {
                    bitmap.Save(chosenFilePath);
                }
            }
        }

        private void ImportFile()
        {
            if (_dialog.OpenFileDialog(out string chosenFilePath))
            {
                // deserialize wordsearch from file
                _wordsearchInstance = Wordsearch.Deserialize(XDocument.Load(chosenFilePath));

                // Update interface
                Words = string.Join(Environment.NewLine, _wordsearchInstance.Words.Select(i => i.Text));
                RaisePropertyChanged(nameof(WordsearchTitle));
                UpdatePreview();
            }
        }

        private void ExportFile()
        {
            if (_dialog.SaveFileDialog(WordsearchTitle, out string chosenFilePath))
            {
                // serialize wordsearch to file
                _wordsearchInstance.Serialize().Save(chosenFilePath);
            }
        }

        private void GenerateRandom()
        {
            if (Wordsearch.TryGenerateRandom(Words.Split(new string[] { Environment.NewLine }, StringSplitOptions.None), out Wordsearch ws))
            {
                _wordsearchInstance.Words = ws.Words;
                UpdatePreview();
            }
            else
            {
                _dialog.ShowMessageBox
                (
                    "We're having trouble fitting all of the words onto the wordsearch. Try reducing the length and number of words.",
                    "Unable to Generate Wordsearch",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );

            }
        }
    }
}
