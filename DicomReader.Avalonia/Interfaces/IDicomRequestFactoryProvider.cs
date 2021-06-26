using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomRequestFactoryProvider
    {
        IDicomRequestFactory ProvideFactory(DicomQueryInputs inputs);
    }
}
