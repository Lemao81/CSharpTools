using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Common.Extensions;
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
        public async Task<TResult> ExecuteDicomQuery<TResult>(DicomQueryInputs queryInputs, PacsConfiguration pacsConfiguration,
            IDicomResponseCollector responseCollector)
        {
            var cts = new CancellationTokenSource();
            var dicomRequest = AvaloniaLocator.CurrentMutable.GetService<IDicomRequestFactory>()
                .CreateRequest(queryInputs, pacsConfiguration, responseCollector, cts, OnResponseReceived);
            var dicomClient = AvaloniaLocator.CurrentMutable.GetService<IDicomClientFactory>().CreateClient(queryInputs.DicomRequestType, pacsConfiguration);
            await SendRequest(queryInputs.DicomRequestType, dicomRequest, dicomClient, pacsConfiguration, cts);
            var responseProcessingStrategy = AvaloniaLocator.Current.GetService<IDicomResponseProcessingStrategy<TResult>>();

            return responseProcessingStrategy.ProcessResponse(responseCollector.ResponseDatasets);
        }

        private static async Task SendRequest(DicomRequestType requestType, DicomRequest dicomRequest, DicomClient dicomClient, PacsConfiguration pacsConfig, CancellationTokenSource cts)
        {
            try
            {
                switch (requestType)
                {
                    case DicomRequestType.None:
                        throw new InvalidOperationException("Request type not specified");
                    case DicomRequestType.CFind:
                    case DicomRequestType.CGet:
                        await dicomClient.AddRequestAsync(dicomRequest);
                        await dicomClient.SendAsync(cts.Token);
                        break;
                    case DicomRequestType.CMove:
                        await SendCMoveRequest(dicomRequest, dicomClient, pacsConfig);
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

        private static void OnResponseReceived(DicomRequest request, DicomResponse response, IDicomResponseCollector responseCollector,
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

        private static async Task SendCMoveRequest(DicomRequest request, DicomClient client, PacsConfiguration pacsConfig, CancellationTokenSource cts)
        {
            var completionSource = new TaskCompletionSource<object>();
            try
            {
                var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Consts.CMoveRequestTimeoutSeconds));
                Task.Run(async () =>
                {
                    using var scpServer = DicomServer.Create<CStoreScp>(pacsConfig.ScpPort);
                    completionSource.SetResult(null);
                    await Task.Delay(-1, timeoutToken.Token);
                }, timeoutToken.Token);
            }
            catch (TaskCanceledException)
            {
                await Console.Error.WriteLineAsync("Scp server has been timed out");
                Environment.Exit(1);
            }
            catch (Exception exception)
            {
                await Console.Error.WriteLineAsync("Error during scp server creation: " + exception.Message);
                Environment.Exit(1);
            }

            await completionSource.Task;
            await client.AddRequestAsync(request);
            await client.SendAsync();
        }
    }
}
