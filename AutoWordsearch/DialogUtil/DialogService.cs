using System.Windows;
using System.IO;

namespace AutoWordsearch.DialogUtil
{
    public class DialogService : IDialogService
    {
        private const string FileExtension = ".ws";
        private const string PngFileExtension = ".png";

        public bool SaveImageDialog(string fileNameWithoutExtension, out string chosenPath)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = PngFileExtension,
                Filter = $"Png File (*{PngFileExtension})|*{PngFileExtension}",
            };
            dialog.FileName = Path.Combine(dialog.InitialDirectory, fileNameWithoutExtension + PngFileExtension);

            bool? result = dialog.ShowDialog();

            chosenPath = dialog.FileName;
            return result == true;
        }

        public bool SaveFileDialog(string fileNameWithoutExtension, out string chosenPath)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = FileExtension,
                Filter = $"Wordsearch Files (*{FileExtension})|*{FileExtension}",

            };
            dialog.FileName = Path.Combine(dialog.InitialDirectory, fileNameWithoutExtension + FileExtension);

            bool? result = dialog.ShowDialog();

            chosenPath = dialog.FileName;
            return result == true;
        }

        public bool OpenFileDialog(out string chosenPath)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = FileExtension,
                Filter = $"Wordsearch Files (*{FileExtension})|*{FileExtension}",

            };
            bool? result = dialog.ShowDialog();

            chosenPath = dialog.FileName;
            return result == true;
        }

        public void ShowMessageBox(string message, string title, MessageBoxButton button, MessageBoxImage image)
        {
            MessageBox.Show(message, title, button, image);
        }

    }
}
