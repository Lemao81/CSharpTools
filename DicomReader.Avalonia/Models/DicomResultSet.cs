using System.Collections.Generic;

namespace DicomReader.Avalonia.Models
{
    public class DicomResultSet
    {
        public DicomResultSet(List<List<DicomResult>> results)
        {
            Results = results;
        }

        public List<List<DicomResult>> Results { get; }
    }
}
