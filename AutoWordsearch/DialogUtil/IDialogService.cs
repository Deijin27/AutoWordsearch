using System.Windows;

namespace AutoWordsearch.DialogUtil
{
    public interface IDialogService
    {
        bool OpenFileDialog(out string chosenPath);
        bool SaveFileDialog(string fileNameWithoutExtension, out string chosenPath);
        bool SaveImageDialog(string fileNameWithoutExtension, out string chosenPath);
        void ShowMessageBox(string message, string title, MessageBoxButton button, MessageBoxImage image);
    }
}