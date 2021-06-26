using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IConfigurationChangedHandler
    {
        void HandleConfigurationChanged(MainWindowViewModel mainWindowViewModel, ConfigurationChangedData changedData);
    }
}
