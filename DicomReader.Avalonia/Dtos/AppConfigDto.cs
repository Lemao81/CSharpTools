using System.Collections.Generic;
using System.Linq;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;
using Newtonsoft.Json;

namespace DicomReader.Avalonia.Dtos
{
    public class AppConfigDto
    {
        public AppConfigDto()
        {
        }

        public AppConfigDto(AppConfig appConfig)
        {
            LastLoadedPacsConfiguration = appConfig.LastLoadedPacsConfiguration;
            OutputFormat = appConfig.OutputFormat.ToString();
            PacsConfigurationDtos = appConfig.PacsConfigurations.Select(c => new PacsConfigurationDto(c));
        }

        public string? LastLoadedPacsConfiguration { get; set; }
        public string? OutputFormat { get; set; }

        [JsonProperty(PropertyName = "PacsConfigurations")]
        public IEnumerable<PacsConfigurationDto>? PacsConfigurationDtos { get; set; }
    }
}
