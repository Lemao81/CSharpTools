using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DicomCrawler.Models
{
    public class PacsServerConfiguration : INotifyPropertyChanged
    {
        private string _host;
        private string _port;
        private string _callingAet;
        private string _calledAet;
        private bool _isReadOnly;

        public string Host
        {
            get => _host;
            set
            {
                if (_host != value)
                {
                    _host = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CallingAet
        {
            get => _callingAet;
            set
            {
                if (_callingAet != value)
                {
                    _callingAet = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CalledAet
        {
            get => _calledAet;
            set
            {
                if (_calledAet != value)
                {
                    _calledAet = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
