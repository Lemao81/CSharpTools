using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomRequestFactoryProvider
    {
        IDicomCFindRequestFactory ProvideFactory(DicomQueryInputs inputs);
    }
}
