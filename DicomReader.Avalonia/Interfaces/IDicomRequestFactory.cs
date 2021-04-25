using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomRequestFactory
    {
        DicomCFindRequest CreateCFindRequest(DicomQueryParams queryParams);
    }
}
