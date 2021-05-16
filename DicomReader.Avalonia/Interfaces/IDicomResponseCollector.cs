using System.Collections.Generic;
using Dicom;
using Dicom.Network;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomResponseCollector
    {
        List<DicomDataset> ResponseDatasets { get; }
        bool CollectResponse(DicomResponse response);
    }
}
