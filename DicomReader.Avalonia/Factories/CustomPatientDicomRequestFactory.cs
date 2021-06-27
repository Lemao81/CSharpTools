using System;
using System.Linq;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class CustomPatientDicomRequestFactory : AbstractCustomDicomRequestFactory
    {
        protected override DicomCFindRequest CreateCustomRequestInternal(DicomQueryParams queryParams)
        {
            if (queryParams.RetrieveLevel != DicomRetrieveLevel.Patient) throw new InvalidOperationException("Retrieve level must be patient");
            
            var patientId = queryParams.PatientId.IsNullOrEmpty() ? null : queryParams.PatientId;

            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Patient);

            request.Dataset.AddOrUpdate(DicomTag.PatientID, patientId);

            return request;
        }
    }
}
