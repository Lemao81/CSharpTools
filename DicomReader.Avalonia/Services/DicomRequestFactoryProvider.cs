using System;
using Dicom.Network;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Factories;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class DicomRequestFactoryProvider : IDicomRequestFactoryProvider
    {
        private readonly IDicomTagProvider _dicomTagProvider;

        public DicomRequestFactoryProvider(IDicomTagProvider dicomTagProvider)
        {
            _dicomTagProvider = dicomTagProvider;
        }

        public IDicomRequestFactory ProvideFactory(DicomQueryInputs inputs)
        {
            if (inputs.DicomRequestType == DicomRequestType.StandardPatient)
                return new StandardPatientDicomRequestFactory();

            if (inputs.DicomRequestType == DicomRequestType.StandardStudy)
                return new StandardStudyDicomRequestFactory();

            if (inputs.DicomRequestType == DicomRequestType.StandardSeries)
                return new StandardSeriesDicomRequestFactory();

            if (inputs.DicomRequestType == DicomRequestType.Custom && inputs.DicomQueryParams.RetrieveLevel == DicomQueryRetrieveLevel.Patient)
                return new CustomPatientDicomRequestFactory(_dicomTagProvider);

            if (inputs.DicomRequestType == DicomRequestType.Custom && inputs.DicomQueryParams.RetrieveLevel == DicomQueryRetrieveLevel.Study)
                return new CustomStudyDicomRequestFactory(_dicomTagProvider);

            if (inputs.DicomRequestType == DicomRequestType.Custom && inputs.DicomQueryParams.RetrieveLevel == DicomQueryRetrieveLevel.Series)
                return new CustomSeriesDicomRequestFactory(_dicomTagProvider);

            throw new InvalidOperationException("Dicom request strategy couldn't be evaluated");
        }
    }
}
