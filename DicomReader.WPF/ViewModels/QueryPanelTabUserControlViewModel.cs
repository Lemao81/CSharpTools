using System;
using System.Collections.ObjectModel;
using Dicom.Network;
using DicomReader.WPF.Extensions;
using DicomReader.WPF.Interfaces;
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
            RequestedFields.CollectionChanged += (s, e) => ClearRequestedFieldsCommand.RaiseCanExecuteChanged();
            AddRequestedFieldCommand = new DelegateCommand(AddRequestedField, CanAddRequestedField).ObservesProperty(() => RequestedField);
            RemoveRequestedFieldCommand = new DelegateCommand(RemoveRequestedField, CanRemoveRequestedField).ObservesProperty(() => SelectedRequestedField);
            ClearRequestedFieldsCommand = new DelegateCommand(ClearRequestedFields, CanClearRequestedFields);
            ExecuteQueryCommand = new DelegateCommand(ExecuteQuery, CanExecuteQuery).ObservesCanExecute(() => IsQueryExecutable);
        }

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

        public string SelectedRequestedField
        {
            get => _selectedRequestedField;
            set => SetProperty(ref _selectedRequestedField, value);
        }

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
            if (_selectedRequestedField.IsNullOrEmpty()) return;

            RequestedFields.Remove(_selectedRequestedField);
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

            await _dicomQueryService.ExecuteDicomQuery(this, pacsConfiguration);
        }

        private bool CanExecuteQuery() =>
            (!PatientId.IsNullOrEmpty() ||
             !StudyInstanceUid.IsNullOrEmpty() ||
             !AccessionNumber.IsNullOrEmpty()) &&
            RequestedFields.Count > 0;
        #endregion
    }
}
