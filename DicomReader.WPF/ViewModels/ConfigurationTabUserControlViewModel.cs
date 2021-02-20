using System;
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
            Initialize();
        }

        public void Initialize()
        {
            LoadAppConfig()
                .OnFailureOrException(InitializeNew)
                .OnSuccess(InitializeWithLoaded);
        }

        #region bound properties
        public string ConfigurationName
        {
            get => _configurationName;
            set => SetProperty(ref _configurationName, value);
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

        public bool IsConfigurationChanged
        {
            get => _isConfigurationChanged;
            set => SetProperty(ref _isConfigurationChanged, value);
        }

        public PacsConfiguration SelectedConfiguration => PacsConfiguration.Create(ConfigurationName, Host, Port, CallingAet, CalledAet).Value;

        #region bound commands
        public DelegateCommand SaveConfigurationCommand { get; }
        #endregion

        private Result<AppConfiguration> LoadAppConfig()
        {
            if (_fileSystemService.FileExists(Consts.ConfigFileName))
            {
                return _fileSystemService.ReadFile(Consts.ConfigFileName)
                    .ShowErrorIfNoSuccess("Reading config file failed")
                    .Select(AppConfiguration.Parse);
            }

            var emptyConfig = AppConfiguration.CreateEmpty();
            emptyConfig.PacsConfigurations["Staging"] = PacsConfiguration.Create("Staging", "192.168.35.50", "4242", "", "ORTHANC").Value;

            return _fileSystemService.WriteFile(Consts.ConfigFileName, emptyConfig.AsIndentedJson())
                .ShowErrorIfNoSuccess("Creating config file failed")
                .Select(emptyConfig);
        }

        private void InitializeWithLoaded(AppConfiguration appConfig)
        {
            if (appConfig.PacsConfigurations.Any())
            {
                var (_, value) = appConfig.PacsConfigurations.First();
                ConfigurationName = value.Name;
                Host = value.Host;
                Port = value.Port.ToString();
                CallingAet = value.CallingAet;
                CalledAet = value.CalledAet;

                ConfigurationChanged?.Invoke(this, new ConfigurationChangedArgs
                {
                    PacsConfiguration = value
                });
            }
            else
            {
                InitializeNew();
            }
        }

        private void InitializeNew()
        {
            ConfigurationName = "Staging";
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
                            appConfig.PacsConfigurations[ConfigurationName] = pacsConfig;

                            return _fileSystemService.WriteFile(Consts.ConfigFileName, appConfig.AsIndentedJson());
                        }))
                .ShowErrorIfNoSuccess("Saving config file failed")
                .OnSuccess(() => _isConfigurationChanged = false);
        }
        #endregion
    }
}
