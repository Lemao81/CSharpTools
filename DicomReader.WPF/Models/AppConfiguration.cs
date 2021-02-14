using System;
using System.Collections.Generic;
using DicomReader.WPF.Extensions;

namespace DicomReader.WPF.Models
{
    public class AppConfiguration
    {
        private AppConfiguration()
        {
        }

        public Dictionary<string, PacsConfiguration> PacsConfigurations { get; set; }

        public static Result<AppConfiguration> Parse(string serializedString)
        {
            try
            {
                var configDeserialized = serializedString.Deserialize<AppConfiguration>();
                configDeserialized.PacsConfigurations ??= new Dictionary<string, PacsConfiguration>();

                return configDeserialized;
            }
            catch (Exception exception)
            {
                return Result<AppConfiguration>.Exception(exception);
            }
        }

        public static AppConfiguration CreateEmpty() => new AppConfiguration
        {
            PacsConfigurations = new Dictionary<string, PacsConfiguration>()
        };
    }
}
