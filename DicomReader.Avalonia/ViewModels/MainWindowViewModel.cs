using System;
using System.Threading.Tasks;
using Avalonia;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Dtos;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
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
            QueryResultViewModel.NextPageRequested += (s, e) => NextPageRequested?.Invoke(s, e);
            // TODO show log messages
            LogEntry.Stream.Subscribe(_ =>
            {
            });
        }

        public static event EventHandler? NextPageRequested;

        public DicomQueryViewModel DicomQueryViewModel { get; }
        public QueryResultViewModel QueryResultViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public AppConfig AppConfig { get; set; } = AppConfig.Empty;

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
            DicomQueryViewModel.StartPagedQuery?.Subscribe(async queryInputs => await HandleStartQuery(queryInputs));
        }

        private void HandleChangedConfigurationData(ConfigurationChangedData changedData) =>
            AvaloniaLocator.Current.GetService<IConfigurationChangedHandler>().HandleConfigurationChanged(this, changedData);

        private void HandleSavePacsConfiguration(PacsConfiguration editedConfiguration) => AvaloniaLocator.Current.GetService<ISavePacsConfigurationHandler>()
            .SavePacsConfiguration(this, editedConfiguration);

        private async Task HandleStartQuery(DicomQueryInputs queryInputs) =>
            await AvaloniaLocator.Current.GetService<IStartQueryHandler>().StartQueryAsync(this, queryInputs);
    }
}
