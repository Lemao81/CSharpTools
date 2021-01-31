using DicomReader2.Bases;

namespace DicomReader2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _host;
        private string _port;
        private string _callingAet;
        private string _calledAet;

        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string CallingAet
        {
            get => _callingAet;
            set => SetProperty(ref _callingAet, value);
        }

        public string CalledAet
        {
            get => _calledAet;
            set => SetProperty(ref _calledAet, value);
        }
    }
}
