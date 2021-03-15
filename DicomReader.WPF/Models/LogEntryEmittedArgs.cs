using System;

namespace DicomReader.WPF.Models
{
    public class LogEntryEmittedArgs : EventArgs
    {
        public LogEntryEmittedArgs(string logEntry)
        {
            LogEntry = logEntry;
        }

        public string LogEntry { get; set; }
    }
}
