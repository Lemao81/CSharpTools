using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomCFindRequestFactoryProvider
    {
        IDicomCFindRequestFactory ProvideFactory(DicomQueryInputs inputs);
    }
}
