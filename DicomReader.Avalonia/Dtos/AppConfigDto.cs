using System.Collections.Generic;
using System.Linq;
using DicomReader.Avalonia.Models;
using Newtonsoft.Json;

namespace DicomReader.Avalonia.Dtos
{
    public class AppConfigDto
    {
        public AppConfigDto(AppConfig appConfig)
        {
            LastLoadedPacsConfiguration = appConfig.LastLoadedPacsConfiguration;
            PacsConfigurationDtos = appConfig.PacsConfigurations.Select(c => new PacsConfigurationDto(c));
        }

        public string? LastLoadedPacsConfiguration { get; set; }

        [JsonProperty(PropertyName = "PacsConfigurations")]
        public IEnumerable<PacsConfigurationDto>? PacsConfigurationDtos { get; set; }
    }
}
