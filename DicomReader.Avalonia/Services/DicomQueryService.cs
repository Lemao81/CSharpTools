using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using Dicom.Network.Client.EventArguments;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using AssociationAcceptedEventArgs = Dicom.Network.Client.EventArguments.AssociationAcceptedEventArgs;
using AssociationRejectedEventArgs = Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.Avalonia.Services
{
    public class DicomQueryService : IDicomQueryService
    {
        public async Task<TResult> ExecuteDicomQuery<TResult>(DicomQueryInputs queryInputs, PacsConfiguration pacsConfiguration, int? take = null)
        {
            var dicomRequestFactoryProvider = AvaloniaLocator.Current.GetService<IDicomRequestFactoryProvider>();
            var requestFactory = dicomRequestFactoryProvider.ProvideFactory(queryInputs);
            var findRequest = requestFactory.CreateCFindRequest(queryInputs.DicomQueryParams);
            var responseDatasets = await SendFindRequest(findRequest, pacsConfiguration, take);
            var responseProcessingStrategy = AvaloniaLocator.Current.GetService<IDicomResponseProcessingStrategy<TResult>>();

            return responseProcessingStrategy.ProcessResponse(responseDatasets);
        }

        private static async Task<List<DicomDataset>> SendFindRequest(DicomCFindRequest findRequest, PacsConfiguration configuration, int? take)
        {
            try
            {
                var callingAe = configuration.CallingAe.IsNullOrEmpty() ? "FINDSCU" : configuration.CallingAe;
                var client = new DicomClient(configuration.Host, configuration.Port, false, callingAe, configuration.CalledAe);
                client.NegotiateAsyncOps();

                var responseCount = 0;
                var responseDatasets = new List<DicomDataset>();
                var cancelToken = new CancellationTokenSource();
                findRequest.OnResponseReceived = (_, response) => OnResponseReceived(response, responseDatasets, ref responseCount, take, cancelToken);
                findRequest.OnTimeout = (_, _) => OnTimeout();
                client.StateChanged += (_, args) => OnStateChanged(args);
                client.AssociationAccepted += (_, args) => OnAssociationAccepted(args);
                client.AssociationRejected += (_, args) => OnAssociationRejected(args);
                client.RequestTimedOut += (_, args) => OnRequestTimedOut(args);
                client.AssociationReleased += (_, _) => OnAssociationReleased();

                await client.AddRequestAsync(findRequest);
                await client.SendAsync(cancelToken.Token);

                return responseDatasets;
            }
            catch (SocketException exception)
            {
                if (!exception.Message.Contains("not properly respond")) throw;

                LogEntry.Emit(new LogEntry("Connection to PACS server could not be established"));

                return Enumerable.Empty<DicomDataset>().ToList();
            }
            catch (Exception exception)
            {
                LogEntry.Emit(new LogEntry($"Exception during dicom query. {exception.Message}"));
                throw new ApplicationException("Dicom query failed", exception);
            }
        }

        private static void OnResponseReceived(DicomCFindResponse response, ICollection<DicomDataset> responseDatasets, ref int responseCount, int? take,
            CancellationTokenSource cancelToken)
        {
            if (!response.HasDataset || response.Dataset == null)
            {
                AuditTrailEntry.Emit($"RESPONSE RECEIVED WITHOUT DATA. Status: {response.Status}");

                return;
            }

            AuditTrailEntry.Emit($"RESPONSE RECEIVED WITH DATA. Status: {response.Status}");
            responseCount++;
            responseDatasets.Add(response.Dataset);

            if (take.HasValue && responseCount >= take.Value) cancelToken.Cancel();
        }

        private static void OnTimeout() => AuditTrailEntry.Emit("REQUEST TIMED OUT");

        private static void OnStateChanged(StateChangedEventArgs args) => AuditTrailEntry.Emit(args?.NewState?.ToString());

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
