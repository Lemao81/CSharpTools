using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Dtos
{
    public class PacsConfigurationDto
    {
        public PacsConfigurationDto(PacsConfiguration pacsConfiguration)
        {
            Name = pacsConfiguration.Name;
            Host = pacsConfiguration.Host;
            Port = pacsConfiguration.Port.ToString();
            CallingAe = pacsConfiguration.CallingAe;
            CalledAe = pacsConfiguration.CalledAe;
        }

        public string? Name { get; set; }
        public string? Host { get; set; }
        public string? Port { get; set; }
        public string? CallingAe { get; set; }
        public string? CalledAe { get; set; }
    }
}
