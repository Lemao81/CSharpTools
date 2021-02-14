using System.Windows;

namespace DicomReader.WPF.Helpers
{
    public static class MessageBoxHelper
    {
        public static void ShowError(string title, string message) => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
