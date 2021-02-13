using System.Collections.ObjectModel;
using Dicom.Network;
using Prism.Mvvm;

namespace DicomReader.WPF.ViewModels
{
    public class QueryPanelTabUserControlViewModel : BindableBase
    {
        private string _patientId;
        private string _studyInstanceUid;
        private string _accessionNumber;
        private DicomQueryRetrieveLevel _retrieveLevel;
        private string _requestedField;
        private string _selectedRequestedField;

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

        public ObservableCollection<string> RequestedFields { get; } = new ObservableCollection<string>();

        public string SelectedRequestedField
        {
            get => _selectedRequestedField;
            set => SetProperty(ref _selectedRequestedField, value);
        }
    }
}
