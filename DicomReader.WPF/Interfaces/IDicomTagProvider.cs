using Dicom;
using DicomReader.WPF.Models;

namespace DicomReader.WPF.Interfaces
{
    public interface IDicomTagProvider
    {
        Result<DicomTag> ProvideDicomTag(string keywordOrHexCode);
    }
}
