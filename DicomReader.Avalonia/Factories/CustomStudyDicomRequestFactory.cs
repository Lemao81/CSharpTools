using System;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class CustomStudyDicomRequestFactory : CustomDicomRequestFactory
    {
        public CustomStudyDicomRequestFactory(IDicomTagProvider dicomTagProvider) : base(dicomTagProvider)
        {
        }

        protected override DicomCFindRequest CreateRequestInternal(DicomQueryParams queryParams)
        {
            if (queryParams.RetrieveLevel != DicomQueryRetrieveLevel.Study) throw new InvalidOperationException("Retrieve level must be study");
            if (queryParams.StudyInstanceUid.IsNullOrEmpty() && queryParams.AccessionNumber.IsNullOrEmpty())
                throw new InvalidOperationException("Standard study query requires either a study instance uid or an accession number");

            var patientId = queryParams.PatientId.IsNullOrEmpty() ? null : queryParams.PatientId;
            var studyInstanceUid = queryParams.StudyInstanceUid.IsNullOrEmpty() ? null : queryParams.StudyInstanceUid;
            var accessionNumber = queryParams.AccessionNumber.IsNullOrEmpty() ? null : queryParams.AccessionNumber;

            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study);

            request.Dataset.AddOrUpdate(DicomTag.PatientID, patientId);
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, studyInstanceUid);
            request.Dataset.AddOrUpdate(DicomTag.AccessionNumber, accessionNumber);

            return request;
        }
    }
}
