using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dicom;
using Dicom.Network;
using DicomReader2.Extensions;
using DicomReader2.ViewModels;
using static System.String;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader2.Services
{
    public class DicomQueryService
    {
        public async Task ExecuteDicomQuery(MainViewModel viewModel)
        {
            await SendFindRequest(CreateFindRequest(viewModel), viewModel);
        }

        private DicomCFindRequest CreateFindRequest(MainViewModel viewModel)
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

            var dicomTagProvider = new DicomTagProvider();
            foreach (var keywordOrHexCode in viewModel.RequestedFields.Select(r => r.Trim()).Where(r => !r.IsNullOrEmpty()))
            {
                dicomTagProvider.ProvideDicomTag(keywordOrHexCode)
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

        private static async Task SendFindRequest(DicomCFindRequest request, MainViewModel viewModel)
        {
            var client = new DicomClient(viewModel.Host, int.Parse(viewModel.Port), false, viewModel.CallingAet, viewModel.CalledAet);
            client.NegotiateAsyncOps();

            var result = new List<DicomDataset>();
            request.OnResponseReceived = (_, response) =>
            {
                if (response.HasDataset && response.Dataset != null)
                {
                    result.Add(response.Dataset);
                }
            };
            client.AssociationRejected += (sender, args) => throw new ApplicationException(args.Reason.ToString());
            client.RequestTimedOut += (sender, args) => throw new ApplicationException(args.ToString());
            client.AssociationReleased += (sender, args) =>
            {
                var x = 3;
            };

            await client.AddRequestAsync(request);
            await client.SendAsync();
        }
    }
}
