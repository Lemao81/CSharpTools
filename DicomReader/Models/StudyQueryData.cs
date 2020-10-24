namespace DicomReader.Models
{
    public class StudyQueryData
    {
        public string PatiendId { get; set; }
        public string StudyInstanceUid { get; set; }
        public string SeriesInstanceUid { get; set; }
    }
}