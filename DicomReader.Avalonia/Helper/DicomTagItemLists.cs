using System.Collections.Generic;
using System.Linq;
using Dicom;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Helper
{
    public static class DicomTagItemLists
    {
        public static readonly IEnumerable<DicomTagItem> StandardPatient = new[]
        {
            new DicomTagItem(nameof(DicomTag.PatientID)),
            new DicomTagItem(nameof(DicomTag.PatientName)),
            new DicomTagItem(nameof(DicomTag.IssuerOfPatientID)),
            new DicomTagItem(nameof(DicomTag.PatientSex)),
            new DicomTagItem(nameof(DicomTag.PatientBirthDate))
        };

        public static readonly IEnumerable<DicomTagItem> StandardStudy = new[]
        {
            new DicomTagItem(nameof(DicomTag.PatientID)),
            new DicomTagItem(nameof(DicomTag.PatientName)),
            new DicomTagItem(nameof(DicomTag.IssuerOfPatientID)),
            new DicomTagItem(nameof(DicomTag.PatientSex)),
            new DicomTagItem(nameof(DicomTag.PatientBirthDate)),
            new DicomTagItem(nameof(DicomTag.StudyInstanceUID)),
            new DicomTagItem(nameof(DicomTag.ModalitiesInStudy)),
            new DicomTagItem(nameof(DicomTag.StudyID)),
            new DicomTagItem(nameof(DicomTag.AccessionNumber)),
            new DicomTagItem(nameof(DicomTag.StudyDate)),
            new DicomTagItem(nameof(DicomTag.StudyTime)),
            new DicomTagItem(nameof(DicomTag.StudyDescription)),
            new DicomTagItem(nameof(DicomTag.NumberOfStudyRelatedSeries)),
            new DicomTagItem(nameof(DicomTag.NumberOfStudyRelatedInstances))
        };

        public static readonly IEnumerable<DicomTagItem> StandardSeries = new[]
        {
            new DicomTagItem(nameof(DicomTag.StudyInstanceUID)),
            new DicomTagItem(nameof(DicomTag.SeriesInstanceUID)),
            new DicomTagItem(nameof(DicomTag.SeriesNumber)),
            new DicomTagItem(nameof(DicomTag.SeriesDescription)),
            new DicomTagItem(nameof(DicomTag.Modality)),
            new DicomTagItem(nameof(DicomTag.SeriesDate)),
            new DicomTagItem(nameof(DicomTag.SeriesTime)),
            new DicomTagItem(nameof(DicomTag.NumberOfSeriesRelatedInstances))
        };

        public static readonly IEnumerable<DicomTagItem> Patients = new[]
        {
            new DicomTagItem(nameof(DicomTag.PatientID)),
            new DicomTagItem(nameof(DicomTag.PatientName)),
            new DicomTagItem(nameof(DicomTag.PatientSex)),
            new DicomTagItem(nameof(DicomTag.PatientBirthDate)),
            new DicomTagItem(nameof(DicomTag.PatientAge)),
            new DicomTagItem(nameof(DicomTag.PatientSize)),
            new DicomTagItem(nameof(DicomTag.PatientWeight))
        };

        public static readonly IEnumerable<DicomTagItem> PatientsExtended = Patients.Concat(new[]
        {
            new DicomTagItem(nameof(DicomTag.PatientAddress)),
            new DicomTagItem(nameof(DicomTag.PatientState)),
            new DicomTagItem(nameof(DicomTag.PatientComments)),
            new DicomTagItem(nameof(DicomTag.PatientOrientation)),
            new DicomTagItem(nameof(DicomTag.PatientPosition))
        });

        public static readonly IEnumerable<DicomTagItem> Study = new[]
        {
            new DicomTagItem(nameof(DicomTag.StudyInstanceUID)),
            new DicomTagItem(nameof(DicomTag.StudyDescription)),
            new DicomTagItem(nameof(DicomTag.StudyDate)),
            new DicomTagItem(nameof(DicomTag.StudyID))
        };

        public static readonly IEnumerable<DicomTagItem> Series = new[]
        {
            new DicomTagItem(nameof(DicomTag.SeriesInstanceUID)),
            new DicomTagItem(nameof(DicomTag.SeriesDescription))
        };

        public static readonly IEnumerable<DicomTagItem> SeriesExtended = Series.Concat(new[]
        {
            new DicomTagItem(nameof(DicomTag.SeriesDate)),
            new DicomTagItem(nameof(DicomTag.SeriesType)),
            new DicomTagItem(nameof(DicomTag.SeriesNumber)),
            new DicomTagItem(nameof(DicomTag.SeriesTime))
        });

        public static readonly IEnumerable<DicomTagItem> PatientStudies = new[]
        {
            new DicomTagItem(nameof(DicomTag.PatientID)),
            new DicomTagItem(nameof(DicomTag.PatientName)),
            new DicomTagItem(nameof(DicomTag.StudyDescription))
        };

        public static readonly IEnumerable<DicomTagItem> DeviceInfos = new[]
        {
            new DicomTagItem(nameof(DicomTag.DeviceSerialNumber)),
            new DicomTagItem(nameof(DicomTag.DeviceUID)),
            new DicomTagItem(nameof(DicomTag.DeviceID)),
            new DicomTagItem(nameof(DicomTag.DeviceDescription)),
            new DicomTagItem(nameof(DicomTag.LongDeviceDescription)),
            new DicomTagItem(nameof(DicomTag.UniqueDeviceIdentifier)),
            new DicomTagItem(nameof(DicomTag.Manufacturer)),
            new DicomTagItem(nameof(DicomTag.SoftwareVersions)),
            new DicomTagItem(nameof(DicomTag.ManufacturerRelatedModelGroup)),
            new DicomTagItem(nameof(DicomTag.ManufacturerModelName)),
            new DicomTagItem(nameof(DicomTag.ManufacturerModelVersion)),
            new DicomTagItem(nameof(DicomTag.ManufacturerDeviceIdentifier)),
            new DicomTagItem(nameof(DicomTag.ManufacturerDeviceClassUID))
        };
    }
}
