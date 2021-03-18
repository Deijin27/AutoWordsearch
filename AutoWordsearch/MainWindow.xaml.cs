using ReactiveUI;
using System.Reactive.Disposables;
using AutoWordsearch.DialogUtil;

namespace AutoWordsearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel()
            {
                SaveImageDialog = Dialogs.SaveImageDialog,
                SaveFileDialog = Dialogs.SaveFileDialog,
                OpenFileDialog = Dialogs.OpenFileDialog,
                ShowMessageBox = Dialogs.ShowMessageBox
            };

            this.WhenActivated(disposable =>
            {

                this.Bind(ViewModel,
                    viewModel => viewModel.WordsearchTitle,
                    view => view.WordsearchTitleTextBox.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    vm => vm.Words,
                    v => v.WordsTextBox.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    vm => vm.PreviewSource,
                    v => v.PreviewImage.Source)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    vm => vm.GenerateRandom,
                    v => v.GenerateRandomButton)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    vm => vm.ExportImage,
                    v => v.ExportImageButton)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    vm => vm.ImportFile,
                    v => v.ImportFileButton)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    vm => vm.ExportFile,
                    v => v.ExportFileButton)
                    .DisposeWith(disposable);


            });
        }
    }
}
