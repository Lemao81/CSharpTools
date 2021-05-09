using System.Collections.Generic;
using System.Collections.ObjectModel;
using DicomReader.Avalonia.Enums;

namespace DicomReader.Avalonia.Models
{
    public class DicomQueryParams
    {
        public DicomQueryParams(string patientId, string studyInstanceUid, string accessionNumber, DicomRetrieveLevel retrieveLevel,
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
        public DicomRetrieveLevel RetrieveLevel { get; }
        public IReadOnlyCollection<DicomTagItem> RequestedDicomTags { get; }
    }
}
