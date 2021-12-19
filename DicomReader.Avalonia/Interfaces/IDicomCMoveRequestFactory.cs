using System.Threading;
using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomCMoveRequestFactory : IDicomRequestFactory
    {
        DicomCMoveRequest CreateCMoveRequest(
            DicomQueryParams        queryParams,
            PacsConfiguration       pacsConfig,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cts);
    }
}
