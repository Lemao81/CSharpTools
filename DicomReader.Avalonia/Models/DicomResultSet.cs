using System.Collections.Generic;

namespace DicomReader.Avalonia.Models
{
    public class DicomResultSet
    {
        public DicomResultSet(List<DicomResult> results)
        {
            Results = results;
        }

        public List<DicomResult> Results { get; }
    }
}
