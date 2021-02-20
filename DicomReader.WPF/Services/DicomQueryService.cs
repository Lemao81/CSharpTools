using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dicom;
using Dicom.Network;
using DicomReader.WPF.Extensions;
using DicomReader.WPF.Interfaces;
using DicomReader.WPF.Models;
using DicomReader.WPF.ViewModels;
using static System.String;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.WPF.Services
{
    public class DicomQueryService : IDicomQueryService
    {
        private readonly IDicomTagProvider _dicomTagProvider;

        public DicomQueryService(IDicomTagProvider dicomTagProvider)
        {
            _dicomTagProvider = dicomTagProvider;
        }

        public async Task<List<List<DicomResult>>> ExecuteDicomQuery(QueryPanelTabUserControlViewModel viewModel, PacsConfiguration pacsConfiguration)
        {
            var findRequest = CreateFindRequest(viewModel);

            return await SendFindRequestAndCollectResult(findRequest, pacsConfiguration);
        }

        private DicomCFindRequest CreateFindRequest(QueryPanelTabUserControlViewModel viewModel)
        {
            var request = new DicomCFindRequest(viewModel.RetrieveLevel);

            switch (viewModel.RetrieveLevel)
            {
                case DicomQueryRetrieveLevel.Patient:
                    request.Dataset.AddOrUpdate(DicomTag.PatientID, viewModel.PatientId ?? Empty);
                    break;
                case DicomQueryRetrieveLevel.Study:
                    request.Dataset.AddOrUpdate(DicomTag.PatientID, viewModel.PatientId ?? Empty);
                    request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, viewModel.StudyInstanceUid ?? Empty);
                    request.Dataset.AddOrUpdate(DicomTag.AccessionNumber, viewModel.AccessionNumber ?? Empty);
                    break;
                case DicomQueryRetrieveLevel.Series:
                    request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, viewModel.StudyInstanceUid ?? Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var keywordOrHexCode in viewModel.RequestedFields.Select(r => r.Trim()).Where(r => !r.IsNullOrEmpty()))
            {
                _dicomTagProvider.ProvideDicomTag(keywordOrHexCode)
                    .OnSuccess(dicomTag => AddDicomTagToRequest(request, dicomTag))
                    .OnException(exception => throw exception);
            }

            return request;
        }

        private static void AddDicomTagToRequest(DicomCFindRequest findRequest, DicomTag tag)
        {
            var accordingEntry = DicomDictionary.Default.FirstOrDefault(_ => _.Tag == tag);
            if (accordingEntry == null) return;

            var vr = accordingEntry.ValueRepresentations.First();
            if (vr.ValueType == null) return;

            var addMethod = typeof(DicomQueryService).GetMethod(nameof(DatasetAddGeneric), BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(vr.ValueType);
            addMethod?.Invoke(null, new object[] { findRequest, tag });
        }

        private static void DatasetAddGeneric<T>(DicomCFindRequest findRequest, DicomTag tag) => findRequest.Dataset.AddOrUpdate<T>(tag);

        private static async Task<List<List<DicomResult>>> SendFindRequestAndCollectResult(DicomCFindRequest findRequest, PacsConfiguration pacsConfiguration)
        {
            try
            {
                var client = new DicomClient(pacsConfiguration.Host, pacsConfiguration.Port, false,
                    pacsConfiguration.CallingAet.IsNullOrEmpty() ? "FINDSCU" : pacsConfiguration.CallingAet,
                    pacsConfiguration.CalledAet);
                client.NegotiateAsyncOps();

                var result = new List<List<DicomResult>>();
                findRequest.OnResponseReceived = (_, response) =>
                {
                    if (!response.HasDataset || response.Dataset == null) return;

                    var dataSetValues = new List<DicomResult>();
                    dataSetValues.AddRange(response.Dataset.Select(entry =>
                        DicomResult.Create(entry.Tag.DictionaryEntry.Keyword, response.Dataset.GetString(entry.Tag))));
                    result.Add(dataSetValues);
                };
                findRequest.OnTimeout = (_, args) =>
                {
                    Console.WriteLine("findRequest.OnTimeout");
                };
                client.StateChanged += (sender, args) =>
                {
                    Console.WriteLine($"State: {args.NewState}");
                };
                client.AssociationAccepted += (sender, args) =>
                {
                    Console.WriteLine("client.AssociationAccepted");
                };
                client.AssociationRejected += (sender, args) =>
                {
                    Console.WriteLine("client.AssociationRejected");
                    throw new ApplicationException(args.Reason.ToString());
                };
                client.RequestTimedOut += (sender, args) =>
                {
                    Console.WriteLine("client.RequestTimedOut");
                    throw new ApplicationException(args.ToString());
                };
                client.AssociationReleased += (sender, args) =>
                {
                    Console.WriteLine("client.AssociationReleased");
                };

                await client.AddRequestAsync(findRequest);
                await client.SendAsync();

                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception during dicom query");
                throw new ApplicationException("Dicom query failed", exception);
            }
        }
    }
}
