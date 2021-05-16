using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Common.Extensions;
using Dicom.Log;
using Dicom.Network;
using Dicom.Network.Client.EventArguments;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using AssociationAcceptedEventArgs = Dicom.Network.Client.EventArguments.AssociationAcceptedEventArgs;
using AssociationRejectedEventArgs = Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.Avalonia.Services
{
    public class DicomQueryService : IDicomQueryService
    {
        public async Task<TResult> ExecuteDicomQuery<TResult>(DicomQueryInputs queryInputs, PacsConfiguration pacsConfiguration,
            IDicomResponseCollector responseCollector)
        {
            // TODO complete
            switch (queryInputs.DicomRequestType)
            {
                case DicomRequestType.CFind:
                    break;
                case DicomRequestType.CGet:
                    break;
            }

            var dicomRequestFactoryProvider = AvaloniaLocator.Current.GetService<IDicomRequestFactoryProvider>();
            var requestFactory = dicomRequestFactoryProvider.ProvideFactory(queryInputs);
            var findRequest = requestFactory.CreateCFindRequest(queryInputs.DicomQueryParams);
            await SendFindRequest(findRequest, pacsConfiguration, responseCollector);
            var responseProcessingStrategy = AvaloniaLocator.Current.GetService<IDicomResponseProcessingStrategy<TResult>>();

            return responseProcessingStrategy.ProcessResponse(responseCollector.ResponseDatasets);
        }

        private static async Task SendFindRequest(DicomCFindRequest findRequest, PacsConfiguration configuration, IDicomResponseCollector responseCollector)
        {
            try
            {
                var callingAe = configuration.CallingAe.IsNullOrEmpty() ? "FINDSCU" : configuration.CallingAe;
                var client = new DicomClient(configuration.Host, configuration.Port, false, callingAe, configuration.CalledAe);
                client.NegotiateAsyncOps();

                var cancelToken = new CancellationTokenSource();
                findRequest.OnResponseReceived = (_, response) => OnResponseReceived(response, responseCollector, cancelToken);
                findRequest.OnTimeout = (_, _) => OnTimeout();
                client.StateChanged += (_, args) => OnStateChanged(args);
                client.AssociationAccepted += (_, args) => OnAssociationAccepted(args);
                client.AssociationRejected += (_, args) => OnAssociationRejected(args);
                client.AssociationReleased += (_, _) => OnAssociationReleased();
                client.RequestTimedOut += (_, args) => OnRequestTimedOut(args);

                await client.AddRequestAsync(findRequest);
                await client.SendAsync(cancelToken.Token);
            }
            catch (SocketException exception)
            {
                if (!exception.Message.Contains("not properly respond")) throw;

                LogEntry.Emit("Connection to PACS server could not be established");
            }
            catch (Exception exception)
            {
                LogEntry.Emit($"Exception during dicom query. {exception.Message}");
                throw new ApplicationException("Dicom query failed", exception);
            }
        }

        private static void OnResponseReceived(DicomResponse response, IDicomResponseCollector responseCollector, CancellationTokenSource cancelToken)
        {
            if (response.Dataset == null || !response.Dataset.Any())
            {
                AuditTrailEntry.Emit($"RESPONSE RECEIVED WITHOUT DATA. Status: {response.Status}");

                return;
            }

            var message = $"RESPONSE RECEIVED WITH DATA. Status: {response.Status}.";
            message += App.IsExtendedLog ? $" Data: {response.Dataset.WriteToString()}" : "";
            AuditTrailEntry.Emit(message);

            var abort = responseCollector.CollectResponse(response);
            if (!abort || cancelToken.IsCancellationRequested) return;

            AuditTrailEntry.Emit("RECEIVING RESPONSES ABORTED");
            cancelToken.Cancel();
        }

        private static void OnTimeout() => AuditTrailEntry.Emit("REQUEST TIMED OUT");

        private static void OnStateChanged(StateChangedEventArgs args) => AuditTrailEntry.Emit(args.NewState?.ToString());

        private static void OnAssociationAccepted(AssociationAcceptedEventArgs args) =>
            AuditTrailEntry.Emit($"ASSOCIATION ACCEPTED. Host: {args.Association.RemoteHost} " +
                                 $"Port: {args.Association.RemotePort} CalledAE: {args.Association.CalledAE} " +
                                 $"CallingAE: {args.Association.CallingAE}");

        private static void OnAssociationRejected(AssociationRejectedEventArgs args)
        {
            AuditTrailEntry.Emit($"ASSOCIATION REJECTED. Reason: {args.Reason}");
            throw new ApplicationException(args.Reason.ToString());
        }

        private static void OnRequestTimedOut(RequestTimedOutEventArgs args)
        {
            AuditTrailEntry.Emit("CLIENT TIMED OUT");
            throw new ApplicationException(args.ToString());
        }

        private static void OnAssociationReleased() => AuditTrailEntry.Emit("ASSOCIATION RELEASED");
    }
}
