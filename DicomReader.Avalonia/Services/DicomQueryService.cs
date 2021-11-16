using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Dicom.Log;
using Dicom.Network;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.Avalonia.Services
{
    public class DicomQueryService : IDicomQueryService
    {
        private readonly IDicomRequestFactory _dicomRequestFactory;
        private readonly IDicomClientFactory  _dicomClientFactory;
        private readonly IDicomTagProvider    _dicomTagProvider;

        public DicomQueryService()
        {
            _dicomRequestFactory = AvaloniaLocator.CurrentMutable.GetService<IDicomRequestFactory>();
            _dicomClientFactory  = AvaloniaLocator.CurrentMutable.GetService<IDicomClientFactory>();
            _dicomTagProvider    = AvaloniaLocator.Current.GetService<IDicomTagProvider>();
        }

        public async Task<TResult> ExecuteDicomQuery<TResult>(
            DicomQueryInputs        queryInputs,
            PacsConfiguration       pacsConfiguration,
            IDicomResponseCollector responseCollector)
        {
            var responseProcessingStrategy = AvaloniaLocator.Current.GetService<IDicomResponseProcessingStrategy<TResult>>();

            var cts     = new CancellationTokenSource();
            var request = _dicomRequestFactory.CreateRequest(queryInputs, pacsConfiguration, responseCollector, cts, OnResponseReceived);
            var client  = _dicomClientFactory.CreateClient(queryInputs.DicomRequestType, pacsConfiguration);
            await SendRequest(
                queryInputs.DicomRequestType,
                queryInputs.DicomQueryParams.RequestedDicomTags,
                responseCollector,
                request,
                client,
                pacsConfiguration,
                cts
            );

            return responseProcessingStrategy.ProcessResponse(responseCollector.ResponseDatasets);
        }

        private async Task SendRequest(
            DicomRequestType                  requestType,
            IReadOnlyCollection<DicomTagItem> requestedTags,
            IDicomResponseCollector           responseCollector,
            DicomRequest                      dicomRequest,
            DicomClient                       dicomClient,
            PacsConfiguration                 pacsConfig,
            CancellationTokenSource           cts)
        {
            try
            {
                switch (requestType)
                {
                    case DicomRequestType.None:
                        throw new InvalidOperationException("Request type not specified");
                    case DicomRequestType.CFind:
                    case DicomRequestType.CGet:
                        await SendQueryRequest(dicomRequest, dicomClient, cts);
                        break;
                    case DicomRequestType.CMove:
                        await SendQueryRetrieveRequest(dicomRequest, requestedTags, responseCollector, dicomClient, pacsConfig, cts);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(requestType));
                }
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

        private static void OnResponseReceived(
            DicomRequest            request,
            DicomResponse           response,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cancelToken)
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

        private static async Task SendQueryRequest(DicomRequest dicomRequest, DicomClient dicomClient, CancellationTokenSource cts)
        {
            await dicomClient.AddRequestAsync(dicomRequest);
            await dicomClient.SendAsync(cts.Token);
        }

        private async Task SendQueryRetrieveRequest(
            DicomRequest                      request,
            IReadOnlyCollection<DicomTagItem> requestedTags,
            IDicomResponseCollector           responseCollector,
            DicomClient                       client,
            PacsConfiguration                 pacsConfig,
            CancellationTokenSource           cts)
        {
            var createStoreScpCompletion = new TaskCompletionSource<object>();
            try
            {
                var timeoutToken        = new CancellationTokenSource(TimeSpan.FromSeconds(Consts.CMoveRequestTimeoutSeconds));
                var combinedCancelToken = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, timeoutToken.Token);
                Task.Run(
                    async () =>
                    {
                        CStoreScp.ResponseCollector = responseCollector;
                        CStoreScp.RequestedDicomTags = requestedTags.Select(t => _dicomTagProvider.ProvideDicomTag(t.Content))
                                                                    .Where(r => r.IsSuccess)
                                                                    .Select(r => r.Value)
                                                                    .ToList();

                        using var scpServer = DicomServer.Create<CStoreScp>(pacsConfig.ScpPort);
                        createStoreScpCompletion.SetResult(null);
                        await Task.Delay(-1, combinedCancelToken.Token);
                    },
                    combinedCancelToken.Token
                );
            }
            catch (TaskCanceledException)
            {
                LogEntry.Emit("Scp server has been timed out");
            }
            catch (Exception exception)
            {
                LogEntry.Emit("Error during scp server creation: " + exception.Message);
            }

            await createStoreScpCompletion.Task;
            await client.AddRequestAsync(request);
            await client.SendAsync(cts.Token);
        }
    }
}
