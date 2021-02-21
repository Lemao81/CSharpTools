using System.Collections.Generic;
using Dicom;

namespace DicomReader.WPF.Constants
{
    public static class RequestedFieldSets
    {
        public static readonly IEnumerable<string> PatientStandardFields = new[]
        {
            nameof(DicomTag.PatientID),
            nameof(DicomTag.PatientName),
            nameof(DicomTag.PatientSex),
            nameof(DicomTag.PatientBirthDate),
            nameof(DicomTag.PatientAge),
            nameof(DicomTag.PatientSize),
            nameof(DicomTag.PatientWeight)
        };

        public static readonly IEnumerable<string> StudyFields = new[]
        {
            nameof(DicomTag.StudyInstanceUID),
            nameof(DicomTag.StudyDescription),
            nameof(DicomTag.StudyDate),
            nameof(DicomTag.StudyID)
        };

        public static readonly IEnumerable<string> SeriesStandardFields = new[]
        {
            nameof(DicomTag.SeriesInstanceUID),
            nameof(DicomTag.SeriesDescription)
        };

        public static readonly IEnumerable<string> PatientExtendedFields = new[]
        {
            nameof(DicomTag.PatientID),
            nameof(DicomTag.PatientName),
            nameof(DicomTag.PatientSex),
            nameof(DicomTag.PatientBirthDate),
            nameof(DicomTag.PatientAge),
            nameof(DicomTag.PatientSize),
            nameof(DicomTag.PatientWeight),
            nameof(DicomTag.PatientAddress),
            nameof(DicomTag.PatientState),
            nameof(DicomTag.PatientComments),
            nameof(DicomTag.PatientOrientation),
            nameof(DicomTag.PatientPosition)
        };

        public static readonly IEnumerable<string> SeriesExtendedFields = new[]
        {
            nameof(DicomTag.SeriesInstanceUID),
            nameof(DicomTag.SeriesDescription),
            nameof(DicomTag.SeriesDate),
            nameof(DicomTag.SeriesType),
            nameof(DicomTag.SeriesNumber),
            nameof(DicomTag.SeriesTime)
        };

        public static readonly IEnumerable<string> PatientsStudiesFields = new[]
        {
            nameof(DicomTag.PatientID),
            nameof(DicomTag.StudyDescription)
        };
    }
}
