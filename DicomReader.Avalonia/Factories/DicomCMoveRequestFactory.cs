using System;
using System.Threading;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class DicomCMoveRequestFactory : IDicomCMoveRequestFactory
    {
        public DicomRequest CreateRequest(
            DicomQueryInputs inputs,
            PacsConfiguration pacsConfiguration,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cts,
            Action<DicomRequest, DicomResponse, IDicomResponseCollector, CancellationTokenSource> responseAction
        ) =>
            CreateCMoveRequest(inputs.DicomQueryParams, pacsConfiguration, responseCollector, cts, responseAction);

        public DicomCMoveRequest CreateCMoveRequest(DicomQueryParams queryParams, PacsConfiguration pacsConfig, IDicomResponseCollector responseCollector,
            CancellationTokenSource cts, Action<DicomRequest, DicomResponse, IDicomResponseCollector, CancellationTokenSource> responseAction)
        {
            var callingAe = string.IsNullOrEmpty(pacsConfig.CallingAe) ? Consts.StoreScp : pacsConfig.CallingAe;

            var patientId = queryParams.PatientId.IsNullOrEmpty() ? null : queryParams.PatientId;
            var studyInstanceUid = queryParams.StudyInstanceUid.IsNullOrEmpty() ? null : queryParams.StudyInstanceUid;
            var accessionNumber = queryParams.AccessionNumber.IsNullOrEmpty() ? null : queryParams.AccessionNumber;

            var request = new DicomCMoveRequest(callingAe, studyInstanceUid);

            request.Dataset.AddOrUpdate(DicomTag.PatientID, patientId);
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, studyInstanceUid);
            request.Dataset.AddOrUpdate(DicomTag.AccessionNumber, accessionNumber);

            request.OnResponseReceived = (req, res) => responseAction(req, res, responseCollector, cts);

            return request;
        }
    }
}
