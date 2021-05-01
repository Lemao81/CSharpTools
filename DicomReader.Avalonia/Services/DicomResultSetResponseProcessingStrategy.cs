using System.Collections.Generic;
using System.Linq;
using Dicom;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class DicomResultSetResponseProcessingStrategy : IDicomResponseProcessingStrategy<DicomResultSet>
    {
        public DicomResultSet ProcessResponse(List<DicomDataset> responseDatasets)
        {
            var dicomResults = new List<DicomResult>();
            foreach (var dataset in responseDatasets)
            {
                foreach (var entry in dataset)
                {
                    dicomResults.Add(new DicomResult
                    {
                        Name = entry.Tag.DictionaryEntry.Name,
                        Keyword = entry.Tag.DictionaryEntry.Keyword,
                        HexCode = $"{entry.Tag.Group:X4}:{entry.Tag.Element:X4}",
                        ValueRepresentation = entry.Tag.DictionaryEntry.ValueRepresentations.FirstOrDefault()?.Name ?? string.Empty,
                        StringValue = dataset.GetString(entry.Tag)
                    });
                }
            }

            return new DicomResultSet(dicomResults);
        }
    }
}
