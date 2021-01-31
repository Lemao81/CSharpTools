using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Dicom.Network;
using DicomReader2.Bases;
using DicomReader2.Extensions;
using DicomReader2.Helpers;
using DicomReader2.Models;
using DicomReader2.Services;
using Newtonsoft.Json;

namespace DicomReader2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _host;
        private string _port;
        private string _callingAet;
        private string _calledAet;
        private bool _isConfigurationChanged;
        private string _patientId;
        private string _studyInstanceUid;
        private string _accessionNumber;
        private string _requestedField;
        private string _selectedRequestedField;
        private ICommand _saveConfiguration;
        private ICommand _addRequestedField;
        private ICommand _removeRequestedField;
        private ICommand _clearRequestedFields;
        private ICommand _executeQuery;
        private readonly DicomQueryService _dicomQueryService;
        private const string ConfigFileName = "config.json";

        public event EventHandler RequestedFieldFocusRequested;

        public MainViewModel()
        {
            _dicomQueryService = new DicomQueryService();
        }

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

        public DicomQueryRetrieveLevel RetrieveLevel { get; set; }

        public ObservableCollection<string> RequestedFields { get; } = new ObservableCollection<string>();

        public string RequestedField
        {
            get => _requestedField;
            set => SetProperty(ref _requestedField, value);
        }

        public string SelectedRequestedField
        {
            get => _selectedRequestedField;
            set => SetProperty(ref _selectedRequestedField, value);
        }
        #endregion

        public ICommand SaveConfiguration
        {
            get => _saveConfiguration ?? (_saveConfiguration = new CommandHandler(OnSaveConfiguration, CanSaveConfiguration));
            set => _saveConfiguration = value;
        }

        public ICommand AddRequestedField
        {
            get => _addRequestedField ?? (_addRequestedField = new CommandHandler(OnAddRequestedField, CanAddRequestedField));
            set => _addRequestedField = value;
        }

        public ICommand RemoveRequestedField
        {
            get => _removeRequestedField ?? (_removeRequestedField = new CommandHandler(OnRemoveRequestedField, CanRemoveRequestedField));
            set => _removeRequestedField = value;
        }

        public ICommand ClearRequestedFields
        {
            get => _clearRequestedFields ?? (_clearRequestedFields = new CommandHandler(OnClearRequestedFields, CanClearRequestedFields));
            set => _clearRequestedFields = value;
        }

        public ICommand ExecuteQuery
        {
            get => _executeQuery ?? (_executeQuery = new CommandHandler(OnExecuteQuery, CanExecuteQuery));
            set => _executeQuery = value;
        }

        private bool CanSaveConfiguration() => _isConfigurationChanged;

        private bool CanAddRequestedField() => !_requestedField.IsNullOrEmpty();

        private bool CanRemoveRequestedField() => !_selectedRequestedField.IsNullOrEmpty();

        private bool CanClearRequestedFields() => RequestedFields.Count > 0;

        private bool CanExecuteQuery() => !_patientId.IsNullOrEmpty() || !_studyInstanceUid.IsNullOrEmpty() ||
                                          !_accessionNumber.IsNullOrEmpty() && RequestedFields.Count > 0 && !_host.IsNullOrEmpty() && !_port.IsNullOrEmpty();

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

        private void OnAddRequestedField()
        {
            RequestedFields.Add(_requestedField);
            RequestedField = string.Empty;
            RequestedFieldFocusRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnRemoveRequestedField() => RequestedFields.Remove(_selectedRequestedField);

        private void OnClearRequestedFields()
        {
            RequestedFields.Clear();
            RequestedFieldFocusRequested?.Invoke(this, EventArgs.Empty);
        }

        private async void OnExecuteQuery()
        {
            await _dicomQueryService.ExecuteDicomQuery(_patientId, RetrieveLevel, _host, _port, _callingAet, _calledAet);
        }
    }
}
