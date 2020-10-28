using System.ComponentModel;
using System.Runtime.CompilerServices;
using DicomCrawler.Helpers;

namespace DicomCrawler.ViewModels
{
    public class DicomQueryParameterViewModel : INotifyPropertyChanged
    {
        private string _patientId;
        private string _accessionNumber;
        private string _studyInstanceUid;

        public string PatientId
        {
            get => _patientId;
            set
            {
                if (_patientId != value)
                {
                    _patientId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AccessionNumber
        {
            get => _accessionNumber;
            set
            {
                if (_accessionNumber != value)
                {
                    _accessionNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StudyInstanceUid
        {
            get => _studyInstanceUid;
            set
            {
                if (_studyInstanceUid != value)
                {
                    _studyInstanceUid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Any => !PatientId.IsNullOrWhiteSpace() || !AccessionNumber.IsNullOrWhiteSpace() || !StudyInstanceUid.IsNullOrWhiteSpace();

        public DicomQueryParameterViewModel()
        {
        }

        public DicomQueryParameterViewModel(DicomQueryParameterViewModel parameter)
        {
            _patientId = parameter.PatientId;
            _accessionNumber = parameter.AccessionNumber;
            _studyInstanceUid = parameter.StudyInstanceUid;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
