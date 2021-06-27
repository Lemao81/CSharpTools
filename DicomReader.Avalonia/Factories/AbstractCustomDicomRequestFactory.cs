using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public abstract class AbstractCustomDicomRequestFactory : AbstractDicomCFindRequestFactory
    {
        protected override DicomCFindRequest CreateCFindRequestInternal(DicomQueryParams queryParams) => CreateCustomRequestInternal(queryParams);

        protected abstract DicomCFindRequest CreateCustomRequestInternal(DicomQueryParams queryParams);
    }
}
