using System.Collections.Generic;
using DicomCrawler.Enums;

namespace DicomCrawler.Models
{
    public class DicomQuery
    {
        public RetrieveLevel RetrieveLevel { get; set; }
        public DicomQueryParameter Parameter { get; set; }
        public string DicomTagInput { get; set; }
        public ISet<string> DicomTags { get; set; }

        public DicomQuery()
        {
            Parameter = new DicomQueryParameter();
            DicomTags = new HashSet<string>();
        }

        public DicomQuery(DicomQuery dicomQuery)
        {
            RetrieveLevel = dicomQuery.RetrieveLevel;
            Parameter = new DicomQueryParameter(dicomQuery.Parameter);
            DicomTagInput = dicomQuery.DicomTagInput;
            DicomTags = new HashSet<string>(dicomQuery.DicomTags);
        }
    }
}
