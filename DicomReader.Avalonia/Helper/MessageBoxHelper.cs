using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace DicomReader.Avalonia.Helper
{
    public static class MessageBoxHelper
    {
        public static void ShowErrorMessage(string message, string title = "Error") =>
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                Icon = Icon.Error,
                ContentTitle = title,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.Ok,
                Style = Style.Windows,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }).Show();

        public static Task<ButtonResult> ShowConfirmationMessage(string message, string title = "Confirmation") =>
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                Icon = Icon.Warning,
                ContentTitle = title,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.OkCancel,
                Style = Style.Windows,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }).Show();
    }
}
