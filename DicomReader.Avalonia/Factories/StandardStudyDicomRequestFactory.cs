using System;
using Common.Extensions;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class StandardStudyDicomRequestFactory : IDicomCFindRequestFactory
    {
        public DicomCFindRequest CreateCFindRequest(DicomQueryParams queryParams)
        {
            if (queryParams.StudyInstanceUid.IsNullOrEmpty() && queryParams.AccessionNumber.IsNullOrEmpty())
                throw new InvalidOperationException("Standard study query requires either a study instance uid or an accession number");

            var patientId = queryParams.PatientId.IsNullOrEmpty() ? null : queryParams.PatientId;
            var studyInstanceUid = queryParams.StudyInstanceUid.IsNullOrEmpty() ? null : queryParams.StudyInstanceUid;
            var accessionNumber = queryParams.AccessionNumber.IsNullOrEmpty() ? null : queryParams.AccessionNumber;

            return DicomCFindRequest.CreateStudyQuery(patientId, studyInstanceUid: studyInstanceUid, accession: accessionNumber);
        }
    }
}
