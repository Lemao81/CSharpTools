using System;
using System.Collections.Generic;
using DicomReader.WPF.Extensions;
using DicomReader.WPF.Models.Dtos;

namespace DicomReader.WPF.Models
{
    public class AppConfiguration
    {
        private AppConfiguration()
        {
            PacsConfigurations = new Dictionary<string, PacsConfiguration>();
        }

        public Dictionary<string, PacsConfiguration> PacsConfigurations { get; protected set; }

        public static Result<AppConfiguration> Parse(string serializedString)
        {
            try
            {
                var dto = serializedString.Deserialize<AppConfigurationDto>();

                return Map(dto);
            }
            catch (Exception exception)
            {
                return Result<AppConfiguration>.Exception(exception);
            }
        }

        public static AppConfiguration CreateEmpty() => new AppConfiguration();

        public static Result<AppConfiguration> Map(AppConfigurationDto dto)
        {
            var appConfig = new AppConfiguration();
            foreach (var (key, value) in dto.PacsConfigurations)
            {
                var pacsConfiguration = PacsConfiguration.Map(value);
                if (pacsConfiguration.IsSuccess)
                {
                    appConfig.PacsConfigurations.Add(key, pacsConfiguration.Value);
                }
            }

            return appConfig;
        }
    }
}
