using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomClientFactory
    {
        DicomClient CreateClient(DicomRequestType requestType, PacsConfiguration pacsConfig);
    }
}
