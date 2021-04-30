using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace DicomReader.Avalonia.Models
{
    public class AuditTrailEntry
    {
        public AuditTrailEntry(string message)
        {
            Message = message;
        }

        public string Message { get; }

        private static readonly List<IObserver<AuditTrailEntry>> AuditTrailObservers = new();

        public static IObservable<AuditTrailEntry> Stream { get; } = Observable.Create<AuditTrailEntry>(observer =>
        {
            AuditTrailObservers.Add(observer);

            return new Disposable(observer);
        });

        public static void Emit(AuditTrailEntry auditTrail) => AuditTrailObservers.ForEach(o => o.OnNext(auditTrail));

        public class Disposable : IDisposable
        {
            private readonly IObserver<AuditTrailEntry> _observer;

            public Disposable(IObserver<AuditTrailEntry> observer) => _observer = observer;

            public void Dispose() => AuditTrailObservers.Remove(_observer);
        }
    }
}
