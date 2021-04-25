using Dicom;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomTagProvider
    {
        Result<DicomTag> ProvideDicomTag(string keywordOrHexCode);
    }
}
