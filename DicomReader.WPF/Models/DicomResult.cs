namespace DicomReader.WPF.Models
{
    public class DicomResult
    {
        public string Keyword { get; set; }
        public string StringValue { get; set; }

        public static DicomResult Create(string keyword, string stringValue) => new DicomResult
        {
            Keyword = keyword,
            StringValue = stringValue
        };
    }
}
