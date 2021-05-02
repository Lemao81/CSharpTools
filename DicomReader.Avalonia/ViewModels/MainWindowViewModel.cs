using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Common.Extensions;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Dtos;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DynamicData;
using ReactiveUI;

namespace DicomReader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MainViewTab _mainViewTab = MainViewTab.Query;

        public MainWindowViewModel()
        {
            DicomQueryViewModel = new DicomQueryViewModel();
            QueryResultViewModel = new QueryResultViewModel();
            ConfigurationViewModel = new ConfigurationViewModel();
            ConfigurationViewModel.ConfigurationChangedStream.Subscribe(HandleChangedConfigurationData);
            // TODO show log messages
            LogEntry.Stream.Subscribe(_ =>
            {
            });
        }

        public DicomQueryViewModel DicomQueryViewModel { get; }
        public QueryResultViewModel QueryResultViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public AppConfig AppConfig { get; protected set; } = AppConfig.Empty;

        public MainViewTab MainViewTab
        {
            get => _mainViewTab;
            set
            {
                this.RaiseAndSetIfChanged(ref _mainViewTab, value);
                this.RaisePropertyChanged(nameof(IsQueryTabSelected));
                this.RaisePropertyChanged(nameof(IsResultTabSelected));
                this.RaisePropertyChanged(nameof(IsConfigurationTabSelected));
            }
        }

        public bool IsQueryTabSelected
        {
            get => MainViewTab == MainViewTab.Query;
            set => MainViewTab = MainViewTab.Query;
        }

        public bool IsResultTabSelected
        {
            get => MainViewTab == MainViewTab.Result;
            set => MainViewTab = MainViewTab.Result;
        }

        public bool IsConfigurationTabSelected
        {
            get => MainViewTab == MainViewTab.Configuration;
            set => MainViewTab = MainViewTab.Configuration;
        }

        public void Initialize()
        {
            CreateConfigFileIfNotExist();
            InitializeAppConfig();
            AddSubscriptions();
            ConfigurationViewModel.Initialize(AppConfig);
        }

        private void CreateConfigFileIfNotExist()
        {
            var fileSystemService = AvaloniaLocator.Current.GetService<IFileSystemService>();
            if (!fileSystemService.FileExists(Consts.AppConfigFileName))
            {
                fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
            }
        }

        private void InitializeAppConfig()
        {
            var appConfigDto = AvaloniaLocator.Current.GetService<IFileSystemService>().ReadFile(Consts.AppConfigFileName).FromJson<AppConfigDto>();
            AppConfig = appConfigDto != null ? new AppConfig(appConfigDto) : AppConfig;
        }

        private void AddSubscriptions()
        {
            ConfigurationViewModel.SavePacsConfiguration?.Subscribe(HandleSavePacsConfiguration);
            DicomQueryViewModel.StartQuery?.Subscribe(async queryInputs => await HandleStartQuery(queryInputs));
        }

        private void HandleChangedConfigurationData(ConfigurationChangedData changedData)
        {
            if (!changedData.LastSelectedPacsConfigurationName.IsNullOrEmpty())
            {
                AppConfig = new AppConfig(AppConfig, changedData.LastSelectedPacsConfigurationName!);
            }

            if (changedData.OutputFormat.HasValue)
            {
                AppConfig = new AppConfig(AppConfig, changedData.OutputFormat.Value);
            }

            SaveAppConfig();
        }

        private void HandleSavePacsConfiguration(PacsConfiguration editedConfiguration)
        {
            AppConfig = new AppConfig(AppConfig, editedConfiguration.Name);
            var existingConfiguration = AppConfig.PacsConfigurations.SingleOrDefault(c => c.Name.EqualsIgnoringCase(editedConfiguration.Name));
            if (existingConfiguration != null)
            {
                AppConfig.PacsConfigurations.Replace(existingConfiguration, editedConfiguration);
            }
            else
            {
                AppConfig.PacsConfigurations.Add(editedConfiguration);
            }

            SaveAppConfig();
            ConfigurationViewModel.Initialize(AppConfig);
            ConfigurationViewModel.SelectedConfiguration = editedConfiguration;
        }

        private async Task HandleStartQuery(DicomQueryInputs queryInputs)
        {
            var dicomQueryService = AvaloniaLocator.Current.GetService<IDicomQueryService>();
            if (ConfigurationViewModel.SelectedConfiguration == null)
                throw new InvalidOperationException("Dicom query started without selected pacs configuration");

            switch (AppConfig.OutputFormat)
            {
                case OutputFormat.JsonSerialized:
                    var serializedString = await dicomQueryService.ExecuteDicomQuery<string>(queryInputs, ConfigurationViewModel.SelectedConfiguration);
                    QueryResultViewModel.Json = serializedString;
                    break;
                case OutputFormat.DicomResult:
                    var resultSet = await dicomQueryService.ExecuteDicomQuery<DicomResultSet>(queryInputs, ConfigurationViewModel.SelectedConfiguration);
                    QueryResultViewModel.Json = resultSet.AsIndentedJson();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(queryInputs));
            }

            MainViewTab = MainViewTab.Result;
        }

        private void SaveAppConfig()
        {
            var fileSystemService = AvaloniaLocator.Current.GetService<IFileSystemService>();
            fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
        }
    }
}
