using System.Collections.ObjectModel;

namespace DicomReader.Avalonia.ViewModels
{
    public class PacsConfigurationViewModel : ViewModelBase
    {
        public ObservableCollection<string> PacsConfigurationNames { get; } = new();
        public string Name { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string CallingAe { get; set; } = string.Empty;
        public string CalledAe { get; set; } = string.Empty;
    }
}
