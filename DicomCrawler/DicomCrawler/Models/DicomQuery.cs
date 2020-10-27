using System.Collections.Generic;
using DicomCrawler.Enums;

namespace DicomCrawler.Models
{
    public class DicomQuery
    {
        public RetrieveLevel RetrieveLevel { get; set; }
        public DicomQueryParameter Parameter { get; set; }
        public ICollection<string> DicomTags { get; set; }
    }
}
