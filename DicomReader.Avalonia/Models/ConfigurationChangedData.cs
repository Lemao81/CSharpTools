using System;
using Common.Extensions;
using DicomReader.Avalonia.Enums;

namespace DicomReader.Avalonia.Models
{
    public class ConfigurationChangedData
    {
        public static ConfigurationChangedData ChangePacsConfiguration(string? lastSelectedPacsConfigurationName) =>
            new()
            {
                LastSelectedPacsConfigurationName = lastSelectedPacsConfigurationName
            };

        public static ConfigurationChangedData ChangeOutputFormat(OutputFormat outputFormat) =>
            new()
            {
                OutputFormat = outputFormat
            };

        public static ConfigurationChangedData RemovePacsConfiguration(string? pacsConfigurationNameToRemove)
        {
            if (pacsConfigurationNameToRemove.IsNullOrEmpty()) throw new InvalidOperationException("Confiugration name to be removed needed");

            return new ConfigurationChangedData
            {
                IsRemoval = true,
                PacsConfigurationNameToRemove = pacsConfigurationNameToRemove
            };
        }


        public string? LastSelectedPacsConfigurationName { get; protected set; }
        public OutputFormat? OutputFormat { get; protected set; }
        public bool? IsRemoval { get; protected set; }
        public string? PacsConfigurationNameToRemove { get; protected set; }
    }
}
