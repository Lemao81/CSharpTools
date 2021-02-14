using System.Collections.Generic;
using DicomReader.WPF.Interfaces;

namespace DicomReader.WPF.Models.Dtos
{
    public class AppConfigurationDto : IDto
    {
        public Dictionary<string, PacsConfigurationDto> PacsConfigurations { get; set; }
    }
}
