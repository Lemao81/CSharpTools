using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Extensions;
using Dicom;
using Dicom.Log;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class CStoreScp : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
    {
        public static List<DicomTag>          RequestedDicomTags = new();
        public static IDicomResponseCollector ResponseCollector  = new UnPagedDicomResponseCollector();

        private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes =
        {
            DicomTransferSyntax.ExplicitVRLittleEndian, DicomTransferSyntax.ExplicitVRBigEndian, DicomTransferSyntax.ImplicitVRLittleEndian,
        };

        private static readonly DicomTransferSyntax[] AcceptedImageTransferSyntaxes =
        {
            // Lossless
            DicomTransferSyntax.JPEGLSLossless,
            DicomTransferSyntax.JPEG2000Lossless,
            DicomTransferSyntax.JPEGProcess14SV1,
            DicomTransferSyntax.JPEGProcess14,
            DicomTransferSyntax.RLELossless,
            // Lossy
            DicomTransferSyntax.JPEGLSNearLossless,
            DicomTransferSyntax.JPEG2000Lossy,
            DicomTransferSyntax.JPEGProcess1,
            DicomTransferSyntax.JPEGProcess2_4,
            // Uncompressed
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private static readonly List<DicomTag> TagsToKeep = new()
        {
            DicomTag.PatientID,
            DicomTag.StudyInstanceUID,
            DicomTag.AccessionNumber,
            DicomTag.SeriesInstanceUID
        };

        private readonly HashSet<string> _distinctSeriesIds = new();

        public CStoreScp(INetworkStream stream, Encoding fallbackEncoding, Logger log) : base(stream, fallbackEncoding, log)
        {
            TagsToKeep.AddRange(RequestedDicomTags);
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            foreach (var context in association.PresentationContexts)
            {
                if (context.AbstractSyntax == DicomUID.Verification)
                {
                    context.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                }
                else if (context.AbstractSyntax.StorageCategory != DicomStorageCategory.None)
                {
                    context.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
                }
            }

            return SendAssociationAcceptAsync(association);
        }

        public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request) => new(request, DicomStatus.Success);

        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            if (request.Dataset == null || !request.HasDataset)
            {
                AuditTrailEntry.Emit("RESPONSE RECEIVED WITHOUT DATA");

                return new DicomCStoreResponse(request, DicomStatus.Success);
            }

            var message = "RESPONSE RECEIVED WITH DATA";
            message += App.IsExtendedLog ? $". Data: {request.Dataset.WriteToString()}" : "";
            AuditTrailEntry.Emit(message);

            var seriesInstanceUid = request.Dataset.GetString(DicomTag.SeriesInstanceUID);
            if (seriesInstanceUid.IsNullOrEmpty() || _distinctSeriesIds.Contains(seriesInstanceUid))
                return new DicomCStoreResponse(request, DicomStatus.Success);

            _distinctSeriesIds.Add(seriesInstanceUid);
            request.Dataset.Remove(i => !TagsToKeep.Contains(i.Tag));
            ResponseCollector.ResponseDatasets.Add(request.Dataset);

            return new DicomCStoreResponse(request, DicomStatus.Success);
        }

        public void OnCStoreRequestException(string tempFileName, Exception exception) => LogEntry.Emit($"Exception on cstore request: {exception.Message}");

        public Task OnReceiveAssociationReleaseRequestAsync() => SendAssociationReleaseResponseAsync();

        public void OnConnectionClosed(Exception? exception)
        {
            if (exception != null)
            {
                LogEntry.Emit($"Exception on connection closed: {exception.Message}");
            }
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason) =>
            LogEntry.Emit($"Scp connection has been aborted. Source: {source}, reason: {reason}");
    }
}
