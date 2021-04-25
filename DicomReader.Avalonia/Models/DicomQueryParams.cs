using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dicom.Network;

namespace DicomReader.Avalonia.Models
{
    public class DicomQueryParams
    {
        public DicomQueryParams(string patientId, string studyInstanceUid, string accessionNumber, DicomQueryRetrieveLevel retrieveLevel,
            IList<DicomTagItem> requestedDicomTags)
        {
            PatientId = patientId;
            StudyInstanceUid = studyInstanceUid;
            AccessionNumber = accessionNumber;
            RetrieveLevel = retrieveLevel;
            RequestedDicomTags = new ReadOnlyCollection<DicomTagItem>(requestedDicomTags);
        }

        public string PatientId { get; }
        public string StudyInstanceUid { get; }
        public string AccessionNumber { get; }
        public DicomQueryRetrieveLevel RetrieveLevel { get; }
        public IReadOnlyCollection<DicomTagItem> RequestedDicomTags { get; }
    }
}
