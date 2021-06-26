using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;

namespace DicomReader.Avalonia.Interfaces
{
    public interface ISavePacsConfigurationHandler
    {
        void SavePacsConfiguration(MainWindowViewModel mainWindowViewModel, PacsConfiguration configuration);
    }
}
