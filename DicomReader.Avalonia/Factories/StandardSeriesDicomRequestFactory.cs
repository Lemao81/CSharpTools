using System;
using Common.Extensions;
using Dicom.Network;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class StandardSeriesDicomRequestFactory : IDicomRequestFactory
    {
        public DicomCFindRequest CreateCFindRequest(DicomQueryParams queryParams)
        {
            if (queryParams.StudyInstanceUid.IsNullOrEmpty())
                throw new InvalidOperationException("Standard series request requires a study instance uid");

            return DicomCFindRequest.CreateSeriesQuery(queryParams.StudyInstanceUid);
        }
    }
}
