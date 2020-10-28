using System;
using DicomCrawler.Models;
using Eto.Forms;

namespace DicomCrawler.Helpers
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string @string) => string.IsNullOrEmpty(@string);
        
        public static bool IsNullOrWhiteSpace(this string @string) => string.IsNullOrWhiteSpace(@string);

        public static Settings GetSettings(this Application application)
        {
            if (!(application.Properties[Settings.SettingsKey] is Settings settings))
            {
                throw new ApplicationException("no settings available");
            }

            return settings;
        }
    }
}
