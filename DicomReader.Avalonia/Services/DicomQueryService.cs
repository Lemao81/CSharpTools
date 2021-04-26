using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using Common.Extensions;
using Dicom.Network;
using Dicom.Network.Client.EventArguments;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using static System.String;
using AssociationAcceptedEventArgs = Dicom.Network.Client.EventArguments.AssociationAcceptedEventArgs;
using AssociationRejectedEventArgs = Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.Avalonia.Services
{
    public class DicomQueryService : IDicomQueryService
    {
        public async Task<List<DicomResultSet>> ExecuteDicomQuery(DicomQueryInputs queryInputs, PacsConfiguration pacsConfiguration)
        {
            var requestFactory = AvaloniaLocator.Current.GetService<IDicomRequestFactoryProvider>().ProvideFactory(queryInputs);
            var findRequest = requestFactory.CreateCFindRequest(queryInputs.DicomQueryParams);

            return await SendFindRequestAndCollectResult(findRequest, pacsConfiguration);
        }

        private static async Task<List<DicomResultSet>> SendFindRequestAndCollectResult(DicomCFindRequest findRequest, PacsConfiguration configuration)
        {
            try
            {
                var callingAe = configuration.CallingAe.IsNullOrEmpty() ? "FINDSCU" : configuration.CallingAe;
                var client = new DicomClient(configuration.Host, configuration.Port, false, callingAe, configuration.CalledAe);
                client.NegotiateAsyncOps();

                var result = new List<DicomResultSet>();
                findRequest.OnResponseReceived = (_, response) => OnResponseReceived(response, result);
                findRequest.OnTimeout = (_, _) => OnTimeout();
                client.StateChanged += (_, args) => OnStateChanged(args);
                client.AssociationAccepted += (_, args) => OnAssociationAccepted(args);
                client.AssociationRejected += (_, args) => OnAssociationRejected(args);
                client.RequestTimedOut += (_, args) => OnRequestTimedOut(args);
                client.AssociationReleased += (_, _) => OnAssociationReleased();

                await client.AddRequestAsync(findRequest);
                await client.SendAsync();

                return result;
            }
            catch (SocketException exception)
            {
                if (!exception.Message.Contains("not properly respond")) throw;

                Log("Connection to PACS server could not be established");

                return Enumerable.Empty<DicomResultSet>().ToList();
            }
            catch (Exception exception)
            {
                Log($"Exception during dicom query. {exception.Message}");
                throw new ApplicationException("Dicom query failed", exception);
            }
        }

        private static void OnResponseReceived(DicomCFindResponse response, List<DicomResultSet> result)
        {
            if (!response.HasDataset || response.Dataset == null)
            {
                Log($"RESPONSE RECEIVED WITHOUT DATA. Status: {response.Status}");
                return;
            }
            Log($"RESPONSE RECEIVED WITH DATA. Status: {response.Status}");

            var dataSetValues = new List<DicomResult>();
            dataSetValues.AddRange(response.Dataset.Select(entry =>
                new DicomResult
                {
                    Name = entry.Tag.DictionaryEntry.Name,
                    Keyword = entry.Tag.DictionaryEntry.Keyword,
                    HexCode = $"{entry.Tag.Group:X4}:{entry.Tag.Element:X4}",
                    ValueRepresentation = entry.Tag.DictionaryEntry.ValueRepresentations.FirstOrDefault()?.Name ?? Empty,
                    StringValue = response.Dataset.GetString(entry.Tag)
                }));
            result.Add(new DicomResultSet(dataSetValues));
        }

        private static void OnTimeout() => Log("REQUEST TIMED OUT");

        private static void OnStateChanged(StateChangedEventArgs args) => Log(args?.NewState?.ToString());

        private static void OnAssociationAccepted(AssociationAcceptedEventArgs args) =>
            Log($"ASSOCIATION ACCEPTED. Host: {args.Association.RemoteHost} " +
                $"Port: {args.Association.RemotePort} CalledAE: {args.Association.CalledAE} " +
                $"CallingAE: {args.Association.CallingAE}");

        private static void OnAssociationRejected(AssociationRejectedEventArgs args)
        {
            Log($"ASSOCIATION REJECTED. Reason: {args.Reason}");
            throw new ApplicationException(args.Reason.ToString());
        }

        private static void OnRequestTimedOut(RequestTimedOutEventArgs args)
        {
            Log("CLIENT TIMED OUT");
            throw new ApplicationException(args.ToString());
        }

        private static void OnAssociationReleased() => Log("ASSOCIATION RELEASED");

        private static void Log(string? text)
        {
            if (!text.IsNullOrEmpty())
            {
                Dispatcher.UIThread.InvokeAsync(() => new LogEntry(text!).Emit());
            }
        }
    }
}
