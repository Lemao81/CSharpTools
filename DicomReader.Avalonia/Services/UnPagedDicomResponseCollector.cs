using Dicom.Network;

namespace DicomReader.Avalonia.Services
{
    public class UnPagedDicomResponseCollector : DicomResponseCollectorBase
    {
        public override bool CollectResponse(DicomResponse response)
        {
            ResponseDatasets.Add(response.Dataset);

            return true;
        }
    }
}
