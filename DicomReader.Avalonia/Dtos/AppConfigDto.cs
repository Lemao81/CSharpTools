using System.Collections.Generic;

namespace DicomReader.Avalonia.Dtos
{
    public class AppConfigDto
    {
        public string? LastLoadedPacsConfiguration { get; set; }
        public IEnumerable<PacsConfigurationDto>? PacsConfigurationDtos { get; set; }
    }
}
