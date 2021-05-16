using System;
using Common.Extensions;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class StandardPatientDicomRequestFactory : IDicomCFindRequestFactory
    {
        public DicomCFindRequest CreateCFindRequest(DicomQueryParams queryParams)
        {
            if (queryParams.PatientId.IsNullOrEmpty()) throw new InvalidOperationException("Standard patient query needs a patient id");

            return DicomCFindRequest.CreatePatientQuery(queryParams.PatientId);
        }
    }
}
