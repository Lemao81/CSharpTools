using System;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Factories;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class DicomRequestFactoryProvider : IDicomRequestFactoryProvider
    {
        public IDicomCFindRequestFactory ProvideFactory(DicomQueryInputs inputs) =>
            inputs.DicomRetrieveType switch
            {
                DicomRetrieveType.StandardPatient => new StandardPatientDicomRequestFactory(),
                DicomRetrieveType.StandardStudy => new StandardStudyDicomRequestFactory(),
                DicomRetrieveType.StandardSeries => new StandardSeriesDicomRequestFactory(),
                DicomRetrieveType.Custom when inputs.DicomQueryParams.RetrieveLevel == DicomRetrieveLevel.Patient => new CustomPatientDicomRequestFactory(),
                DicomRetrieveType.Custom when inputs.DicomQueryParams.RetrieveLevel == DicomRetrieveLevel.Study => new CustomStudyDicomRequestFactory(),
                DicomRetrieveType.Custom when inputs.DicomQueryParams.RetrieveLevel == DicomRetrieveLevel.Series => new CustomSeriesDicomRequestFactory(),
                _ => throw new InvalidOperationException("Dicom request factory couldn't be evaluated")
            };
    }
}
