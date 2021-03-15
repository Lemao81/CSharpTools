using System;

namespace DicomReader.WPF.Models.Event
{
    public class LogEntryEmittedEventArgs : EventArgs
    {
        public LogEntryEmittedEventArgs(string logEntry)
        {
            LogEntry = logEntry;
        }

        public string LogEntry { get; set; }
    }
}
