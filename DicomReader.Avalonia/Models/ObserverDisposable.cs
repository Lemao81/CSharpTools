using System;
using System.Collections.Generic;

namespace DicomReader.Avalonia.Models
{
    public class ObserverDisposable<T> : IDisposable
    {
        private readonly IObserver<T> _observer;
        private readonly List<IObserver<T>> _observers;

        public ObserverDisposable(IObserver<T> observer, List<IObserver<T>> observers)
        {
            _observer = observer;
            _observers = observers;
        }

        public void Dispose() => _observers.Remove(_observer);
    }
}
