using System;
using Common.Extensions;
using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class StandardPatientDicomRequestFactory : AbstractDicomCFindRequestFactory
    {
        protected override DicomCFindRequest CreateCFindRequestInternal(DicomQueryParams queryParams)
        {
            if (queryParams.PatientId.IsNullOrEmpty()) throw new InvalidOperationException("Standard patient query needs a patient id");

            return DicomCFindRequest.CreatePatientQuery(queryParams.PatientId);
        }
    }
}
