using System.Collections.Generic;

namespace DicomReader.Avalonia.Models
{
    public class AppConfig
    {
        public string LastLoadedPacsConfiguration { get; } = string.Empty;
        public IEnumerable<PacsConfiguration> PacsConfigurations { get; } = new List<PacsConfiguration>();
    }
}
