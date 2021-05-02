using DicomReader.Avalonia.Enums;

namespace DicomReader.Avalonia.Models
{
    public class ConfigurationChangedData
    {
        public ConfigurationChangedData(string? lastSelectedPacsConfigurationName)
        {
            LastSelectedPacsConfigurationName = lastSelectedPacsConfigurationName;
        }

        public ConfigurationChangedData(OutputFormat? outputFormat)
        {
            OutputFormat = outputFormat;
        }

        public string? LastSelectedPacsConfigurationName { get; }
        public OutputFormat? OutputFormat { get; }
    }
}
