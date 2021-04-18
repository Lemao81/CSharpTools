using System;
using System.Collections.Generic;
using System.Linq;
using DicomReader.Avalonia.Dtos;

namespace DicomReader.Avalonia.Models
{
    public class AppConfig
    {
        private AppConfig()
        {
        }

        public AppConfig(AppConfigDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            LastLoadedPacsConfiguration = dto.LastLoadedPacsConfiguration ?? string.Empty;
            if (dto.PacsConfigurationDtos != null)
            {
                PacsConfigurations = dto.PacsConfigurationDtos.Select(c => new PacsConfiguration(c)).ToList();
            }
        }

        public static AppConfig Empty = new();

        public string LastLoadedPacsConfiguration { get; } = string.Empty;
        public List<PacsConfiguration> PacsConfigurations { get; } = new();
    }
}
