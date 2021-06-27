using System;
using System.Threading;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public abstract class AbstractDicomCFindRequestFactory : IDicomCFindRequestFactory
    {
        public DicomRequest CreateRequest(
            DicomQueryInputs inputs,
            PacsConfiguration pacsConfiguration,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cts,
            Action<DicomRequest, DicomResponse, IDicomResponseCollector, CancellationTokenSource> responseAction
        ) =>
            CreateCFindRequest(inputs.DicomQueryParams, responseCollector, cts, responseAction);

        public DicomCFindRequest CreateCFindRequest(
            DicomQueryParams queryParams,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cts,
            Action<DicomRequest, DicomResponse, IDicomResponseCollector, CancellationTokenSource> responseAction
        )
        {
            var request = CreateCFindRequestInternal(queryParams);

            request.OnResponseReceived = (req, res) => responseAction(req, res, responseCollector, cts);

            return request;
        }

        protected abstract DicomCFindRequest CreateCFindRequestInternal(DicomQueryParams queryParams);
    }
}
