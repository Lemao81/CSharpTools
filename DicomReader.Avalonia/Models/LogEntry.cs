using System;
using System.Collections.Generic;
using System.Linq;
using DicomReader.Avalonia.Extensions;

namespace DicomReader.Avalonia.Models
{
    public class LogEntry
    {
        public LogEntry(string message, params object[] payloads)
        {
            Message = message;
            CreationDate = DateTime.Now;

            try
            {
                Payloads = payloads.Select(p => p.AsJson());
            }
            catch
            {
                // ignore
            }
        }

        public string Message { get; }
        public IEnumerable<string>? Payloads { get; }
        public DateTime CreationDate { get; }

        public override string ToString()
        {
            var text = $"{CreationDate:yyyy.MM.dd H:mm:ss zzz} - {Message}";
            if (Payloads?.Any() == true)
            {
                text += $" | {string.Join(" - ", Payloads)}";
            }

            return text;
        }
    }
}
