using DicomCrawler.Helpers;

namespace DicomCrawler.Models
{
    public class DicomQueryParameter
    {
        public string PatientId { get; set; }
        public string AccessionNumber { get; set; }
        public string StudyInstanceUid { get; set; }
        public bool Any => !PatientId.IsNullOrEmpty() || !AccessionNumber.IsNullOrEmpty() || !StudyInstanceUid.IsNullOrEmpty();
    }
}
