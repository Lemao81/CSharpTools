using System;
using System.Threading;
using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomCFindRequestFactory : IDicomRequestFactory
    {
        DicomCFindRequest CreateCFindRequest(
            DicomQueryParams queryParams,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cts,
            Action<DicomRequest, DicomResponse, IDicomResponseCollector, CancellationTokenSource> responseAction
        );
    }
}
