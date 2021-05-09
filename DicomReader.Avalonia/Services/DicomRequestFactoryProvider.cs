using System;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Factories;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class DicomRequestFactoryProvider : IDicomRequestFactoryProvider
    {
        public IDicomRequestFactory ProvideFactory(DicomQueryInputs inputs) =>
            inputs.DicomRequestType switch
            {
                DicomRequestType.StandardPatient => new StandardPatientDicomRequestFactory(),
                DicomRequestType.StandardStudy => new StandardStudyDicomRequestFactory(),
                DicomRequestType.StandardSeries => new StandardSeriesDicomRequestFactory(),
                DicomRequestType.Custom when inputs.DicomQueryParams.RetrieveLevel == DicomRetrieveLevel.Patient => new CustomPatientDicomRequestFactory(),
                DicomRequestType.Custom when inputs.DicomQueryParams.RetrieveLevel == DicomRetrieveLevel.Study => new CustomStudyDicomRequestFactory(),
                DicomRequestType.Custom when inputs.DicomQueryParams.RetrieveLevel == DicomRetrieveLevel.Series => new CustomSeriesDicomRequestFactory(),
                _ => throw new InvalidOperationException("Dicom request factory couldn't be evaluated")
            };
    }
}
