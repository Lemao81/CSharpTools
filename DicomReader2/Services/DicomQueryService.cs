using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dicom;
using Dicom.Network;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader2.Services
{
    public class DicomQueryService
    {
        public async Task ExecuteDicomQuery(string patientId, DicomQueryRetrieveLevel retrieveLevel, string host, string port, string callingAet,
            string calledAet)
        {
            switch (retrieveLevel)
            {
                case DicomQueryRetrieveLevel.Patient:
                    var findRequest = DicomCFindRequest.CreatePatientQuery(patientId);
                    await SendFindRequest(findRequest, host, int.Parse(port), callingAet, calledAet);
                    break;
                case DicomQueryRetrieveLevel.Study:
                    break;
                case DicomQueryRetrieveLevel.Series:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(retrieveLevel), retrieveLevel, null);
            }
        }

        private static async Task SendFindRequest(DicomCFindRequest request, string host, int port, string callingAet, string calledAet)
        {
            var client = new DicomClient(host, port, false, callingAet, calledAet);
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
