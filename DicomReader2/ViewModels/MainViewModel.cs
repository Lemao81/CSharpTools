using System.IO;
using System.Windows.Input;
using DicomReader2.Bases;
using DicomReader2.Helpers;
using DicomReader2.Models;
using Newtonsoft.Json;

namespace DicomReader2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _host;
        private string _port;
        private string _callingAet;
        private string _calledAet;
        private ICommand _saveConfiguration;
        private bool _isConfigurationChanged;
        private string _patientId;
        private string _studyInstanceUid;
        private string _accessionNumber;
        private const string ConfigFileName = "config.json";

        public void Init()
        {
            if (File.Exists(ConfigFileName))
            {
                var config = JsonConvert.DeserializeObject<PacsConfiguration>(File.ReadAllText(ConfigFileName));
                Host = config.Host;
                Port = config.Port;
                CallingAet = config.CallingAet;
                CalledAet = config.CalledAet;
            }
        }

        #region PacsConfig
        public string Host
        {
            get => _host;
            set
            {
                _isConfigurationChanged = true;
                SetProperty(ref _host, value);
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                _isConfigurationChanged = true;
                SetProperty(ref _port, value);
            }
        }

        public string CallingAet
        {
            get => _callingAet;
            set
            {
                _isConfigurationChanged = true;
                SetProperty(ref _callingAet, value);
            }
        }

        public string CalledAet
        {
            get => _calledAet;
            set
            {
                _isConfigurationChanged = true;
                SetProperty(ref _calledAet, value);
            }
        }
        #endregion

        #region QueryData
        public string PatientId
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        public string StudyInstanceUid
        {
            get => _studyInstanceUid;
            set => SetProperty(ref _studyInstanceUid, value);
        }

        public string AccessionNumber
        {
            get => _accessionNumber;
            set => SetProperty(ref _accessionNumber, value);
        }
        #endregion

        public ICommand SaveConfiguration
        {
            get => _saveConfiguration ?? (_saveConfiguration = new CommandHandler(OnSaveConfiguration, CanSaveConfiguration));
            set => _saveConfiguration = value;
        }

        private bool CanSaveConfiguration() => _isConfigurationChanged;

        private void OnSaveConfiguration()
        {
            var pacsConfiguration = new PacsConfiguration
            {
                Host = _host,
                Port = _port,
                CallingAet = _callingAet,
                CalledAet = _calledAet
            };
            File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(pacsConfiguration));
            _isConfigurationChanged = false;
        }
    }
}
