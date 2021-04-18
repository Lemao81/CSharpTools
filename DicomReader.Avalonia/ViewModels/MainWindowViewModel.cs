using System;
using System.Linq;
using Common.Extensions;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Dtos;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DynamicData;

namespace DicomReader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IFileSystemService _fileSystemService;

        public MainWindowViewModel(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            DicomQueryViewModel = new DicomQueryViewModel();
            QueryResultViewModel = new QueryResultViewModel();
            PacsConfigurationViewModel = new PacsConfigurationViewModel();
        }

        public DicomQueryViewModel DicomQueryViewModel { get; }
        public QueryResultViewModel QueryResultViewModel { get; }
        public PacsConfigurationViewModel PacsConfigurationViewModel { get; }
        public AppConfig AppConfig { get; protected set; } = new();

        public void Initialize()
        {
            CreateConfigFileIfNotExist();
            PacsConfigurationViewModel.SavePacsConfiguration?.Subscribe(editedConfiguration =>
            {
                var existingConfiguration = AppConfig.PacsConfigurations.SingleOrDefault(c => c.Name.EqualsIgnoringCase(editedConfiguration.Name));
                if (existingConfiguration != null)
                {
                    AppConfig.PacsConfigurations.Replace(existingConfiguration, editedConfiguration);
                }
                else
                {
                    AppConfig.PacsConfigurations.Add(editedConfiguration);
                }

                WriteAppconfigToFile();
            });
        }

        private void CreateConfigFileIfNotExist()
        {
            if (!_fileSystemService.FileExists(Consts.AppConfigFileName))
            {
                _fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
            }

            AppConfig = _fileSystemService.ReadFile(Consts.AppConfigFileName).FromJson<AppConfig>() ?? AppConfig;
        }

        private void WriteAppconfigToFile() => _fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
    }
}
