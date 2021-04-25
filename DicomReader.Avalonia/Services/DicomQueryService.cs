using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Common.Extensions;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;
using static System.String;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.Avalonia.Services
{
    public class DicomQueryService : IDicomQueryService
    {
        private readonly IDicomRequestFactoryProvider _dicomRequestFactoryProvider;

        public DicomQueryService(IDicomRequestFactoryProvider dicomRequestFactoryProvider)
        {
            _dicomRequestFactoryProvider = dicomRequestFactoryProvider;
            var blubb = Observable.Create<LogEntry>((s, b) => );
        }

        public IObservable<LogEntry> LogEntryStream { get; set; }

        public async Task<List<DicomResultSet>> ExecuteDicomQuery(DicomQueryInputs queryInputs, PacsConfiguration pacsConfiguration)
        {
            var requestFactory = _dicomRequestFactoryProvider.ProvideFactory(queryInputs);
            var findRequest = requestFactory.CreateCFindRequest(queryInputs.DicomQueryParams);

            return await SendFindRequestAndCollectResult(findRequest, queryInputs, pacsConfiguration);
        }

        private static async Task<List<DicomResultSet>> SendFindRequestAndCollectResult(DicomCFindRequest findRequest, DicomQueryInputs queryParameter,
            PacsConfiguration pacsConfiguration)
        {
            try
            {
                var client = new DicomClient(pacsConfiguration.Host, pacsConfiguration.Port, false,
                    pacsConfiguration.CallingAe.IsNullOrEmpty() ? "FINDSCU" : pacsConfiguration.CallingAe, pacsConfiguration.CalledAe);
                client.NegotiateAsyncOps();

                var result = new List<DicomResultSet>();
                findRequest.OnResponseReceived = (_, response) =>
                {
                    if (!response.HasDataset || response.Dataset == null)
                    {
                        EmitLogEntry($"RESPONSE RECEIVED WITHOUT DATA. Status: {response.Status}");
                        return;
                    }
                    EmitLogEntry($"RESPONSE RECEIVED WITH DATA. Status: {response.Status}");

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
                };
                findRequest.OnTimeout = (_, _) => EmitLogEntry("REQUEST TIMED OUT");
                client.StateChanged += (_, args) => EmitLogEntry(args?.NewState?.ToString());
                client.AssociationAccepted += (_, args) => EmitLogEntry($"ASSOCIATION ACCEPTED. Host: {args.Association.RemoteHost} " +
                                                                        $"Port: {args.Association.RemotePort} CalledAE: {args.Association.CalledAE} " +
                                                                        $"CallingAE: {args.Association.CallingAE}");
                client.AssociationRejected += (_, args) =>
                {
                    EmitLogEntry($"ASSOCIATION REJECTED. Reason: {args.Reason}");
                    throw new ApplicationException(args.Reason.ToString());
                };
                client.RequestTimedOut += (_, args) =>
                {
                    EmitLogEntry("CLIENT TIMED OUT");
                    throw new ApplicationException(args.ToString());
                };
                client.AssociationReleased += (_, _) => EmitLogEntry("ASSOCIATION RELEASED");

                await client.AddRequestAsync(findRequest);
                await client.SendAsync();

                return result;
            }
            catch (SocketException exception)
            {
                if (!exception.Message.Contains("not properly respond")) throw;

                EmitLogEntry("Connection to PACS server could not be established");

                return Enumerable.Empty<DicomResultSet>().ToList();
            }
            catch (Exception exception)
            {
                EmitLogEntry($"Exception during dicom query. {exception.Message}");
                throw new ApplicationException("Dicom query failed", exception);
            }
        }

        private static void EmitLogEntry(string? logEntry) => DispatchOnUiThread(() => MainWindowViewModel.EmitLogEntry(logEntry));

        private static void DispatchOnUiThread(Action action) => Dispatcher.UIThread.InvokeAsync(action);
    }
}
