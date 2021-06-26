using System.Linq;
using Common.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;

namespace DicomReader.Avalonia.Handler
{
    public class ConfigurationChangedHandler : IConfigurationChangedHandler
    {
        public void HandleConfigurationChanged(MainWindowViewModel mainWindowViewModel, ConfigurationChangedData changedData)
        {
            if (!changedData.LastSelectedPacsConfigurationName.IsNullOrEmpty())
            {
                mainWindowViewModel.AppConfig = new AppConfig(mainWindowViewModel.AppConfig, changedData.LastSelectedPacsConfigurationName!);
            }

            if (changedData.OutputFormat.HasValue)
            {
                mainWindowViewModel.AppConfig = new AppConfig(mainWindowViewModel.AppConfig, changedData.OutputFormat.Value);
            }

            if (changedData.IsRemoval.HasValue && changedData.IsRemoval.Value && !changedData.PacsConfigurationNameToRemove.IsNullOrEmpty())
            {
                mainWindowViewModel.AppConfig = new AppConfig(mainWindowViewModel.AppConfig, string.Empty);
                mainWindowViewModel.AppConfig.PacsConfigurations.Remove(
                    mainWindowViewModel.AppConfig.PacsConfigurations.Single(c => c.Name == changedData.PacsConfigurationNameToRemove));
            }

            mainWindowViewModel.AppConfig.Save();
        }
    }
}
