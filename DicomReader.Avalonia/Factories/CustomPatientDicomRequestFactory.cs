using System;
using System.Linq;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class CustomPatientDicomRequestFactory : CustomDicomRequestFactory
    {
        public CustomPatientDicomRequestFactory(IDicomTagProvider dicomTagProvider) : base(dicomTagProvider)
        {
        }

        protected override DicomCFindRequest CreateRequestInternal(DicomQueryParams queryParams)
        {
            if (queryParams.PatientId.IsNullOrEmpty()) throw new InvalidOperationException("Standard patient query needs a patient id");
            if (queryParams.RetrieveLevel != DicomQueryRetrieveLevel.Patient) throw new InvalidOperationException("Retrieve level must be patient");
            if (queryParams.RequestedDicomTags.Any(t => t.Content.IsNullOrEmpty()))
                throw new InvalidOperationException("Requested dicom tags must contain content");

            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Patient);

            request.Dataset.AddOrUpdate(DicomTag.PatientID, queryParams.PatientId);

            return request;
        }
    }
}
