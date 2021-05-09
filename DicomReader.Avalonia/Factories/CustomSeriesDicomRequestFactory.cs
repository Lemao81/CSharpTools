﻿using System;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class CustomSeriesDicomRequestFactory : CustomDicomRequestFactory
    {
        protected override DicomCFindRequest CreateRequestInternal(DicomQueryParams queryParams)
        {
            if (queryParams.RetrieveLevel != DicomRetrieveLevel.Series) throw new InvalidOperationException("Retrieve level must be series");
            if (queryParams.StudyInstanceUid.IsNullOrEmpty())
                throw new InvalidOperationException("Standard series request requires a study instance uid");

            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Series);

            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, queryParams.StudyInstanceUid);

            return request;
        }
    }
}
