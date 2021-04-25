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
        private readonly IDicomQueryService _dicomQueryService;

        public MainWindowViewModel(IFileSystemService fileSystemService, IDicomQueryService dicomQueryService)
        {
            _fileSystemService = fileSystemService;
            _dicomQueryService = dicomQueryService;
            DicomQueryViewModel = new DicomQueryViewModel();
            QueryResultViewModel = new QueryResultViewModel();
            PacsConfigurationViewModel = new PacsConfigurationViewModel();
            // TODO show log messages
            LogEntry.Stream.Subscribe(_ =>
            {
            });
        }

        public DicomQueryViewModel DicomQueryViewModel { get; }
        public QueryResultViewModel QueryResultViewModel { get; }
        public PacsConfigurationViewModel PacsConfigurationViewModel { get; }
        public AppConfig AppConfig { get; protected set; } = AppConfig.Empty;

        public void Initialize()
        {
            CreateConfigFileIfNotExist();
            InitializeAppConfig();
            AddSubscriptions();
            PacsConfigurationViewModel.Initialize(AppConfig);
        }

        private void CreateConfigFileIfNotExist()
        {
            if (!_fileSystemService.FileExists(Consts.AppConfigFileName))
            {
                _fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
            }
        }

        private void InitializeAppConfig()
        {
            var appConfigDto = _fileSystemService.ReadFile(Consts.AppConfigFileName).FromJson<AppConfigDto>();
            AppConfig = appConfigDto != null ? new AppConfig(appConfigDto) : AppConfig;
        }

        private void AddSubscriptions()
        {
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
                PacsConfigurationViewModel.Initialize(AppConfig);
                PacsConfigurationViewModel.SelectedConfiguration = editedConfiguration;
            });
            DicomQueryViewModel.StartQuery.Subscribe(async queryInputs =>
            {
                if (PacsConfigurationViewModel.SelectedConfiguration == null)
                    throw new InvalidOperationException("Dicom query started without selected pacs configuration");

                await _dicomQueryService.ExecuteDicomQuery(queryInputs, PacsConfigurationViewModel.SelectedConfiguration);
            });
        }

        private void WriteAppconfigToFile() => _fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
    }
}
