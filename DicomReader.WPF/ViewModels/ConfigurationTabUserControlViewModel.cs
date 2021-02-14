using System.IO;
using System.Windows;
using DicomReader.WPF.Constants;
using DicomReader.WPF.Extensions;
using DicomReader.WPF.Helpers;
using DicomReader.WPF.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;

namespace DicomReader.WPF.ViewModels
{
    public class ConfigurationTabUserControlViewModel : BindableBase
    {
        private string _host;
        private string _port;
        private string _callingAet;
        private string _calledAet;
        private bool _isConfigurationChanged;

        public ConfigurationTabUserControlViewModel()
        {
            SaveConfigurationCommand = new DelegateCommand(SaveConfiguration).ObservesCanExecute(() => IsConfigurationChanged);
        }

        #region bound properties
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

        #region bound commands
        public DelegateCommand SaveConfigurationCommand { get; }
        #endregion

        #region command handlers
        private void SaveConfiguration()
        {
            var result = PacsConfiguration.Create(this);
            if (result.IsFailure)
            {
                MessageBoxHelper.ShowError("Invalid input", "Some Input values are invalid");

                return;
            }

            File.WriteAllText(Consts.ConfigFileName, result.Value.AsIndentedJson());
            _isConfigurationChanged = false;
        }
        #endregion
    }
}
