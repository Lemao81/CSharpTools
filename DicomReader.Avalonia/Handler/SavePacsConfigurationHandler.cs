using System.Linq;
using Common.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;
using DynamicData;

namespace DicomReader.Avalonia.Handler
{
    public class SavePacsConfigurationHandler : ISavePacsConfigurationHandler
    {
        public void SavePacsConfiguration(MainWindowViewModel mainWindowViewModel, PacsConfiguration configuration)
        {
            mainWindowViewModel.AppConfig = new AppConfig(mainWindowViewModel.AppConfig, configuration.Name);
            var configViewModel = mainWindowViewModel.ConfigurationViewModel;
            var appConfig = mainWindowViewModel.AppConfig;

            var existingConfiguration = appConfig.PacsConfigurations.SingleOrDefault(c => c.Name.EqualsIgnoringCase(configuration.Name));
            if (existingConfiguration != null)
            {
                appConfig.PacsConfigurations.Replace(existingConfiguration, configuration);
            }
            else
            {
                appConfig.PacsConfigurations.Add(configuration);
            }

            appConfig.Save();
            configViewModel.Initialize(appConfig);
            configViewModel.SelectedConfiguration = configuration;
        }
    }
}
