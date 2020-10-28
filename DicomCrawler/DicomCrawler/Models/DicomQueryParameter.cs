using DicomCrawler.Helpers;

namespace DicomCrawler.Models
{
    public class DicomQueryParameter
    {
        public string PatientId { get; set; }
        public string AccessionNumber { get; set; }
        public string StudyInstanceUid { get; set; }
        public bool Any => !PatientId.IsNullOrWhiteSpace() || !AccessionNumber.IsNullOrWhiteSpace() || !StudyInstanceUid.IsNullOrWhiteSpace();

        public DicomQueryParameter()
        {
        }

        public DicomQueryParameter(DicomQueryParameter parameter)
        {
            PatientId = parameter.PatientId;
            AccessionNumber = parameter.AccessionNumber;
            StudyInstanceUid = parameter.StudyInstanceUid;
        }
    }
}
