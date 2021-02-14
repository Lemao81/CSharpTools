using DicomReader.WPF.Interfaces;

namespace DicomReader.WPF.Models.Dtos
{
    public class PacsConfigurationDto : IDto
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string CallingAet { get; set; }
        public string CalledAet { get; set; }
    }
}
