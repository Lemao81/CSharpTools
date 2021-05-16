using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomCFindRequestFactory
    {
        DicomCFindRequest CreateCFindRequest(DicomQueryParams queryParams);
    }
}
