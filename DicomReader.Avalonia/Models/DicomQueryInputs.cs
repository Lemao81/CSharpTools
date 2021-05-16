using System;
using System.Collections.Generic;
using Common.Extensions;
using DicomReader.Avalonia.Enums;

namespace DicomReader.Avalonia.Models
{
    public class DicomQueryInputs
    {
        public DicomQueryInputs(DicomRetrieveType retrieveType, DicomRetrieveLevel retrieveLevel, DicomRequestType requestType, string patientId,
            string studyInstanceUid, string accessionNumber, IList<DicomTagItem> requestedDicomTags, bool isPagedQuery, string? pageSizeString, int page)
        {
            if (retrieveType == DicomRetrieveType.None) throw new InvalidOperationException("Dicom retrieve type required");

            if (requestType == DicomRequestType.None) throw new InvalidOperationException("Dicom request type required");

            if (!ValidRetrieveLevel(retrieveLevel)) throw new InvalidOperationException("Dicom retrieve level required/not valid");

            var pageSize = -1;
            if (!pageSizeString.IsNullOrEmpty() && (!int.TryParse(pageSizeString, out pageSize) || pageSize <= 0))
                throw new InvalidOperationException("Page size must be positive integer");

            switch (retrieveType)
            {
                case DicomRetrieveType.StandardPatient:
                    if (patientId.IsNullOrEmpty()) throw new InvalidOperationException("Standard patient request requires a patient id");

                    break;
                case DicomRetrieveType.StandardStudy:
                    if (studyInstanceUid.IsNullOrEmpty() && accessionNumber.IsNullOrEmpty())
                        throw new InvalidOperationException("Standard study request requires a study instance uid or accession number");

                    break;
                case DicomRetrieveType.StandardSeries:
                    if (studyInstanceUid.IsNullOrEmpty())
                        throw new InvalidOperationException("Standard series request requires a study instance uid");

                    break;
                case DicomRetrieveType.Custom:
                    if (retrieveLevel == DicomRetrieveLevel.Patient && patientId.IsNullOrEmpty())
                        throw new InvalidOperationException("Patient id required for retrieve level patient");

                    if (retrieveLevel == DicomRetrieveLevel.Study && studyInstanceUid.IsNullOrEmpty() && accessionNumber.IsNullOrEmpty())
                        throw new InvalidOperationException("Retrieve level study requires a study instance uid or accession number");

                    if (retrieveLevel == DicomRetrieveLevel.Series && studyInstanceUid.IsNullOrEmpty())
                        throw new InvalidOperationException("Retrieve level series requires a study instance uid");

                    break;
            }

            DicomRetrieveType = retrieveType;
            DicomRequestType = requestType;
            DicomQueryParams = new DicomQueryParams(patientId, studyInstanceUid, accessionNumber, retrieveLevel, requestedDicomTags);
            PagedQueryParams = new PagedQueryParams(isPagedQuery, pageSize, page);
        }

        public DicomRetrieveType DicomRetrieveType { get; }
        public DicomRequestType DicomRequestType { get; set; }
        public DicomQueryParams DicomQueryParams { get; }
        public PagedQueryParams PagedQueryParams { get; }

        private static bool ValidRetrieveLevel(DicomRetrieveLevel retrieveLevel) =>
            retrieveLevel is DicomRetrieveLevel.Patient or DicomRetrieveLevel.Study or DicomRetrieveLevel.Series;
    }
}
