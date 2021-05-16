using System.Collections.Generic;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;

namespace DicomReader.Avalonia.Services
{
    public abstract class DicomResponseCollectorBase : IDicomResponseCollector
    {
        public List<DicomDataset> ResponseDatasets { get; } = new();

        public abstract bool CollectResponse(DicomResponse response);
    }
}
