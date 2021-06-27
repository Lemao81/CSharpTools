using System;
using Common.Extensions;
using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class StandardSeriesDicomRequestFactory : AbstractDicomCFindRequestFactory
    {
        protected override DicomCFindRequest CreateCFindRequestInternal(DicomQueryParams queryParams)
        {
            if (queryParams.StudyInstanceUid.IsNullOrEmpty())
                throw new InvalidOperationException("Standard series request requires a study instance uid");

            return DicomCFindRequest.CreateSeriesQuery(queryParams.StudyInstanceUid);
        }
    }
}
