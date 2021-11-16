using System;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Dtos
{
    public class PacsConfigurationDto
    {
        public PacsConfigurationDto()
        {
        }

        public PacsConfigurationDto(PacsConfiguration pacsConfiguration)
        {
            if (pacsConfiguration == null) throw new ArgumentNullException(nameof(pacsConfiguration));

            Name      = pacsConfiguration.Name;
            Host      = pacsConfiguration.Host;
            Port      = pacsConfiguration.Port.ToString();
            CallingAe = pacsConfiguration.CallingAe;
            CalledAe  = pacsConfiguration.CalledAe;
            ScpPort   = pacsConfiguration.ScpPort.ToString();
        }

        public string? Name      { get; set; }
        public string? Host      { get; set; }
        public string? Port      { get; set; }
        public string? CallingAe { get; set; }
        public string? CalledAe  { get; set; }
        public string? ScpPort   { get; set; }
    }
}
