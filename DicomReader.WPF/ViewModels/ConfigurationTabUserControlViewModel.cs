using System;
using System.Collections.ObjectModel;
using System.Linq;
using DicomReader.WPF.Constants;
using DicomReader.WPF.Extensions;
using DicomReader.WPF.Interfaces;
using DicomReader.WPF.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace DicomReader.WPF.ViewModels
{
    public class ConfigurationTabUserControlViewModel : BindableBase
    {
        private readonly IFileSystemService _fileSystemService;
        private string _host;
        private string _port;
        private string _callingAet;
        private string _calledAet;
        private bool _isConfigurationChanged;
        private string _configurationName;

        public static event EventHandler<ConfigurationChangedArgs> ConfigurationChanged;

        public ConfigurationTabUserControlViewModel(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            SaveConfigurationCommand = new DelegateCommand(SaveConfiguration).ObservesCanExecute(() => IsConfigurationChanged);
            ChangeConfigurationCommand = new DelegateCommand<string>(ChangeConfiguration);
            ConfigurationNames = new ObservableCollection<string>();
            Initialize();
        }

        public void Initialize()
        {
            LoadAppConfig()
                .OnFailureOrException(InitializeNew)
                .OnSuccess(InitializeWithLoaded);
            IsConfigurationChanged = false;
        }

        public AppConfiguration AppConfiguration { get; set; }

        public bool IsConfigurationChanged
        {
            get => _isConfigurationChanged;
            set => SetProperty(ref _isConfigurationChanged, value);
        }

        #region bound properties
        public ObservableCollection<string> ConfigurationNames { get; set; }

        public string ConfigurationName
        {
            get => _configurationName;
            set
            {
                SetProperty(ref _configurationName, value);
                IsConfigurationChanged = true;
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                if (SetProperty(ref _host, value))
                {
                    IsConfigurationChanged = true;
                }
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                if (SetProperty(ref _port, value))
                {
                    IsConfigurationChanged = true;
                }
            }
        }

        public string CallingAet
        {
            get => _callingAet;
            set
            {
                if (SetProperty(ref _callingAet, value))
                {
                    IsConfigurationChanged = true;
                }
            }
        }

        public string CalledAet
        {
            get => _calledAet;
            set
            {
                if (SetProperty(ref _calledAet, value))
                {
                    IsConfigurationChanged = true;
                }
            }
        }
        #endregion

        #region bound commands
        public DelegateCommand SaveConfigurationCommand { get; }
        public DelegateCommand<string> ChangeConfigurationCommand { get; }
        public DelegateCommand<string> ChangeConfigurationNameCommand { get; }
        #endregion

        private Result<AppConfiguration> LoadAppConfig()
        {
            if (_fileSystemService.FileExists(Consts.ConfigFileName))
            {
                return _fileSystemService.ReadFile(Consts.ConfigFileName)
                    .ShowErrorIfNoSuccess("Reading config file failed")
                    .Select(AppConfiguration.Parse);
            }

            InitializeNew();

            return _fileSystemService.WriteFile(Consts.ConfigFileName, AppConfiguration.AsIndentedJson())
                .ShowErrorIfNoSuccess("Creating config file failed")
                .Select(AppConfiguration);
        }

        private void InitializeWithLoaded(AppConfiguration appConfig)
        {
            if (appConfig.PacsConfigurations.Any())
            {
                var configuration = appConfig.LastLoadedConfiguration.IsNullOrEmpty()
                    ? appConfig.PacsConfigurations.First()
                    : appConfig.PacsConfigurations.Single(c => c.Name == appConfig.LastLoadedConfiguration);
                SetAppAndSelectedConfiguration(appConfig, configuration);
            }
            else
            {
                InitializeNew();
            }
        }

        private void InitializeNew()
        {
            var appConfig = AppConfiguration.CreateEmpty();
            var configuration = PacsConfiguration.Create("Staging", "192.168.35.50", "4242", "", "ORTHANC").Value;
            appConfig.PacsConfigurations.Add(configuration);

            SetAppAndSelectedConfiguration(appConfig, configuration);
        }

        private void SetAppAndSelectedConfiguration(AppConfiguration appConfig, PacsConfiguration selectedConfiguration)
        {
            AppConfiguration = appConfig;
            ConfigurationNames.Clear();
            ConfigurationNames.AddRange(AppConfiguration.PacsConfigurations.Select(c => c.Name));
            SetConfiguration(selectedConfiguration);
        }

        private void SetConfiguration(PacsConfiguration configuration)
        {
            ConfigurationName = configuration.Name;
            Host = configuration.Host;
            Port = configuration.Port.ToString();
            CalledAet = configuration.CalledAet;
            CallingAet = configuration.CallingAet;

            ConfigurationChanged?.Invoke(this, new ConfigurationChangedArgs(configuration));
        }

        #region command handlers
        private void SaveConfiguration()
        {
            PacsConfiguration.Create(this)
                .ShowErrorIfNoSuccess("Some Input values are invalid")
                .Select(pacsConfig =>
                    LoadAppConfig()
                        .Select(appConfig =>
                        {
                            var configToUpdate = appConfig.PacsConfigurations.SingleOrDefault(c => c.Name == pacsConfig.Name);
                            if (configToUpdate != null)
                            {
                                var index = appConfig.PacsConfigurations.IndexOf(configToUpdate);
                                appConfig.PacsConfigurations.RemoveAt(index);
                                appConfig.PacsConfigurations.Insert(index, pacsConfig);
                            }
                            else
                            {
                                appConfig.PacsConfigurations.Add(pacsConfig);
                            }
                            appConfig.LastLoadedConfiguration = pacsConfig.Name;

                            return _fileSystemService.WriteFile(Consts.ConfigFileName, appConfig.AsIndentedJson());
                        }))
                .ShowErrorIfNoSuccess("Saving config file failed")
                .OnSuccess(() => _isConfigurationChanged = false);
        }

        private void ChangeConfiguration(string configurationName)
        {
            var configuration = AppConfiguration.PacsConfigurations.SingleOrDefault(c => c.Name == configurationName);
            if (configuration == null) return;

            SetConfiguration(configuration);
            SaveConfiguration();
            IsConfigurationChanged = false;
        }
        #endregion
    }
}
