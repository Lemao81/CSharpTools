using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomRequestFactory
    {
        DicomRequest CreateRequest(DicomQueryInputs inputs);
    }
}
