using System.Collections.Generic;
using Dicom;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomResponseProcessingStrategy<out T>
    {
        T ProcessResponse(List<DicomDataset> responseDatasets);
    }
}
