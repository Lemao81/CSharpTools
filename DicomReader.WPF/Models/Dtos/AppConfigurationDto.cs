using System.Collections.Generic;
using DicomReader.WPF.Interfaces;

namespace DicomReader.WPF.Models.Dtos
{
    public class AppConfigurationDto : IDto
    {
        public IEnumerable<PacsConfigurationDto> PacsConfigurations { get; set; }
        public string LastLoadedConfiguration { get; set; }
    }
}
