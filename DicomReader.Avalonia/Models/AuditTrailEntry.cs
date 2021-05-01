using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Common.Extensions;

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

            return new ObserverDisposable<AuditTrailEntry>(observer, AuditTrailObservers);
        });

        public static void Emit(AuditTrailEntry auditTrail) => AuditTrailObservers.ForEach(o => o.OnNext(auditTrail));

        public static void Emit(string? message)
        {
            if (!message.IsNullOrEmpty())
            {
                Emit(new AuditTrailEntry(message));
            }
        }
    }
}
