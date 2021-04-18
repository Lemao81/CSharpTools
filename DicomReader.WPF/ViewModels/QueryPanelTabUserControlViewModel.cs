using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dicom.Network;
using DicomReader.WPF.Constants;
using DicomReader.WPF.Extensions;
using DicomReader.WPF.Interfaces;
using DicomReader.WPF.Models.Event;
using Prism.Commands;
using Prism.Mvvm;

namespace DicomReader.WPF.ViewModels
{
    public class QueryPanelTabUserControlViewModel : BindableBase
    {
        private readonly IDicomQueryService _dicomQueryService;
        private string _patientId;
        private string _studyInstanceUid;
        private string _accessionNumber;
        private DicomQueryRetrieveLevel _retrieveLevel;
        private string _requestedField;
        private string _selectedRequestedField;
        private bool _isRequestedFieldFocused;

        public QueryPanelTabUserControlViewModel(IDicomQueryService dicomQueryService)
        {
            _dicomQueryService = dicomQueryService;
            RequestedFields = new ObservableCollection<string>();
            LogEntries = new ObservableCollection<string>();
            SelectedRequestedFields = new List<string>();
            RetrieveLevel = DicomQueryRetrieveLevel.Study;
            RequestedFields.CollectionChanged += (s, e) => ClearRequestedFieldsCommand.RaiseCanExecuteChanged();
            MainWindowViewModel.LogEntryEmitted += (s, e) => LogEntries.Add(e.LogEntry);

            AddRequestedFieldCommand = new DelegateCommand(AddRequestedField, CanAddRequestedField).ObservesProperty(() => RequestedField);
            RemoveRequestedFieldCommand = new DelegateCommand(RemoveRequestedField, CanRemoveRequestedField).ObservesProperty(() => SelectedRequestedField);
            ClearRequestedFieldsCommand = new DelegateCommand(ClearRequestedFields, CanClearRequestedFields);
            ExecuteQueryCommand = new DelegateCommand(ExecuteQuery, CanExecuteQuery).ObservesCanExecute(() => IsQueryExecutable);
            AddPatientStandardFieldsCommand = new DelegateCommand(AddPatientStandardFields);
            AddStudyFieldsCommand = new DelegateCommand(AddStudyFields);
            AddSeriesStandardFieldsCommand = new DelegateCommand(AddSeriesStandardFields);
            AddPatientExtendedFieldsCommand = new DelegateCommand(AddPatientExtendedFields);
            AddSeriesExtendedFieldsCommand = new DelegateCommand(AddSeriesExtendedFields);
            AddPatientsStudiesFieldsCommand = new DelegateCommand(AddPatientsStudiesFields);
            AddDeviceInfoFieldsCommand = new DelegateCommand(AddDeviceInfoFields);
            TestingCommand = new DelegateCommand(Testing);
        }

        public static event EventHandler<ResultChangedEventArgs> ResultChanged;

        #region bound properties
        public string PatientId
        {
            get => _patientId;
            set
            {
                RaisePropertyChanged(nameof(IsQueryExecutable));
                SetProperty(ref _patientId, value);
            }
        }

        public string StudyInstanceUid
        {
            get => _studyInstanceUid;
            set
            {
                RaisePropertyChanged(nameof(IsQueryExecutable));
                SetProperty(ref _studyInstanceUid, value);
            }
        }

        public string AccessionNumber
        {
            get => _accessionNumber;
            set
            {
                RaisePropertyChanged(nameof(IsQueryExecutable));
                SetProperty(ref _accessionNumber, value);
            }
        }

        public DicomQueryRetrieveLevel RetrieveLevel
        {
            get => _retrieveLevel;
            set => SetProperty(ref _retrieveLevel, value);
        }

        public string RequestedField
        {
            get => _requestedField;
            set => SetProperty(ref _requestedField, value);
        }

        public ObservableCollection<string> RequestedFields { get; }
        public ObservableCollection<string> LogEntries { get; }

        public string SelectedRequestedField
        {
            get => _selectedRequestedField;
            set => SetProperty(ref _selectedRequestedField, value);
        }

        public List<string> SelectedRequestedFields { get; }

        public bool IsRequestedFieldFocused
        {
            get => _isRequestedFieldFocused;
            set => SetProperty(ref _isRequestedFieldFocused, value);
        }
        #endregion

        public bool IsQueryExecutable => CanExecuteQuery();

        #region bound commands
        public DelegateCommand AddRequestedFieldCommand { get; }
        public DelegateCommand RemoveRequestedFieldCommand { get; }
        public DelegateCommand ClearRequestedFieldsCommand { get; }
        public DelegateCommand ExecuteQueryCommand { get; }
        public DelegateCommand AddPatientStandardFieldsCommand { get; }
        public DelegateCommand AddStudyFieldsCommand { get; }
        public DelegateCommand AddSeriesStandardFieldsCommand { get; }
        public DelegateCommand AddPatientExtendedFieldsCommand { get; }
        public DelegateCommand AddSeriesExtendedFieldsCommand { get; }
        public DelegateCommand AddPatientsStudiesFieldsCommand { get; }
        public DelegateCommand AddDeviceInfoFieldsCommand { get; }
        public DelegateCommand TestingCommand { get; }
        #endregion

        #region command handlers
        private void AddRequestedField()
        {
            if (_requestedField.IsNullOrEmpty()) return;

            RequestedFields.Add(_requestedField);
            RequestedField = string.Empty;
            IsRequestedFieldFocused = true;
            RaisePropertyChanged(nameof(IsQueryExecutable));
        }

        private bool CanAddRequestedField() => !_requestedField.IsNullOrEmpty();

        private void RemoveRequestedField()
        {
            if (_selectedRequestedField.IsNullOrEmpty() && !SelectedRequestedFields.Any()) return;

            if (!_selectedRequestedField.IsNullOrEmpty())
            {
                RequestedFields.Remove(_selectedRequestedField);
            }

            if (SelectedRequestedFields.Any())
            {
                var selectedFields = new List<string>(SelectedRequestedFields.Where(f => RequestedFields.Contains(f)));
                foreach (var field in selectedFields)
                {
                    RequestedFields.Remove(field);
                }
            }
            RaisePropertyChanged(nameof(IsQueryExecutable));
        }

        private bool CanRemoveRequestedField() => !_selectedRequestedField.IsNullOrEmpty();

        private void ClearRequestedFields()
        {
            if (RequestedFields.IsNullOrEmpty()) return;

            RequestedFields.Clear();
            IsRequestedFieldFocused = true;
            RaisePropertyChanged(nameof(IsQueryExecutable));
        }

        private bool CanClearRequestedFields() => RequestedFields.Count > 0;

        private async void ExecuteQuery()
        {
            var pacsConfiguration = MainWindowViewModel.SelectedConfiguration;
            if (pacsConfiguration == null)
            {
                throw new InvalidOperationException("No PACS configuration available");
            }

            var dicomResults = await _dicomQueryService.ExecuteDicomQuery(this, pacsConfiguration);
            if (!dicomResults.IsNullOrEmpty())
            {
                ResultChanged?.Invoke(this, new ResultChangedEventArgs(dicomResults));
                MainWindowViewModel.EmitSwitchTab(1);
            }
        }

        private bool CanExecuteQuery() => RequestedFields.Count > 0;

        private void AddPatientStandardFields() => AddToRequestedFieldsIfNotExistant(RequestedFieldSets.PatientStandardFields);

        private void AddStudyFields() => AddToRequestedFieldsIfNotExistant(RequestedFieldSets.StudyFields);

        private void AddSeriesStandardFields() => AddToRequestedFieldsIfNotExistant(RequestedFieldSets.SeriesStandardFields);

        private void AddPatientExtendedFields() => AddToRequestedFieldsIfNotExistant(RequestedFieldSets.PatientExtendedFields);

        private void AddSeriesExtendedFields() => AddToRequestedFieldsIfNotExistant(RequestedFieldSets.SeriesExtendedFields);

        private void AddPatientsStudiesFields() => AddToRequestedFieldsIfNotExistant(RequestedFieldSets.PatientsStudiesFields);

        private void AddDeviceInfoFields() => AddToRequestedFieldsIfNotExistant(RequestedFieldSets.DeviceInfoFields);

        private void Testing()
        {
            StudyInstanceUid = "1.2.276.0.33.1.0.4.192.168.56.148.20200331.1192945.88622.2";
            RetrieveLevel = DicomQueryRetrieveLevel.Series;
            AddPatientStandardFields();
            AddStudyFields();
            AddSeriesStandardFields();
        }
        #endregion

        private void AddToRequestedFieldsIfNotExistant(IEnumerable<string> fieldsToAdd)
        {
            foreach (var field in fieldsToAdd)
            {
                if (!RequestedFields.Contains(field))
                {
                    RequestedFields.Add(field);
                }
            }
            RaisePropertyChanged(nameof(IsQueryExecutable));
        }
    }
}
