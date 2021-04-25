using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

        private static readonly List<IObserver<LogEntry>> LogEntryObservers = new();

        public static IObservable<LogEntry> Stream { get; } = Observable.Create<LogEntry>(observer =>
        {
            LogEntryObservers.Add(observer);

            return new Disposable(observer);
        });

        public static void Emit(LogEntry logEntry) => LogEntryObservers.ForEach(o => o.OnNext(logEntry));

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

        public class Disposable : IDisposable
        {
            private readonly IObserver<LogEntry> _observer;

            public Disposable(IObserver<LogEntry> observer) => _observer = observer;

            public void Dispose() => LogEntryObservers.Remove(_observer);
        }
    }
}
