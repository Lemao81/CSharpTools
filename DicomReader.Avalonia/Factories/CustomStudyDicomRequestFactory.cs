﻿using System;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class CustomStudyDicomRequestFactory : AbstractCustomDicomRequestFactory
    {
        protected override DicomCFindRequest CreateCustomRequestInternal(DicomQueryParams queryParams)
        {
            if (queryParams.RetrieveLevel != DicomRetrieveLevel.Study) throw new InvalidOperationException("Retrieve level must be study");

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
