using System;
using System.Collections.Generic;
using Common.Extensions;
using DicomReader.Avalonia.Enums;

namespace DicomReader.Avalonia.Models
{
    public class DicomQueryInputs
    {
        public DicomQueryInputs(DicomRequestType requestType, DicomRetrieveLevel retrieveLevel, string patientId, string studyInstanceUid,
            string accessionNumber, IList<DicomTagItem> requestedDicomTags, bool isPagedQuery, int? pageSize)
        {
            if (requestType == DicomRequestType.None) throw new InvalidOperationException("Dicom request type required");

            if (!ValidRetrieveLevel(retrieveLevel)) throw new InvalidOperationException("Dicom retrieve level required/not valid");

            switch (requestType)
            {
                case DicomRequestType.StandardPatient:
                    if (patientId.IsNullOrEmpty()) throw new InvalidOperationException("Standard patient request requires a patient id");

                    break;
                case DicomRequestType.StandardStudy:
                    if (studyInstanceUid.IsNullOrEmpty() && accessionNumber.IsNullOrEmpty())
                        throw new InvalidOperationException("Standard study request requires a study instance uid or accession number");

                    break;
                case DicomRequestType.StandardSeries:
                    if (studyInstanceUid.IsNullOrEmpty())
                        throw new InvalidOperationException("Standard series request requires a study instance uid");

                    break;
                case DicomRequestType.Custom:
                    if (retrieveLevel == DicomRetrieveLevel.Patient && patientId.IsNullOrEmpty())
                        throw new InvalidOperationException("Patient id required for retrieve level patient");

                    if (retrieveLevel == DicomRetrieveLevel.Study && studyInstanceUid.IsNullOrEmpty() && accessionNumber.IsNullOrEmpty())
                        throw new InvalidOperationException("Retrieve level study requires a study instance uid or accession number");

                    if (retrieveLevel == DicomRetrieveLevel.Series && studyInstanceUid.IsNullOrEmpty())
                        throw new InvalidOperationException("Retrieve level series requires a study instance uid");

                    break;
            }

            DicomRequestType = requestType;
            DicomQueryParams = new DicomQueryParams(patientId, studyInstanceUid, accessionNumber, retrieveLevel, requestedDicomTags);
            PagedQueryParams = new PagedQueryParams(isPagedQuery, pageSize);
        }

        public DicomRequestType DicomRequestType { get; }
        public DicomQueryParams DicomQueryParams { get; }
        public PagedQueryParams PagedQueryParams { get; }

        private static bool ValidRetrieveLevel(DicomRetrieveLevel retrieveLevel) =>
            retrieveLevel == DicomRetrieveLevel.Patient ||
            retrieveLevel == DicomRetrieveLevel.Study ||
            retrieveLevel == DicomRetrieveLevel.Series;
    }
}
