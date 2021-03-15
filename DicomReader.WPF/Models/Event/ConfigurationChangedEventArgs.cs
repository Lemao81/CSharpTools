using System;

namespace DicomReader.WPF.Models.Event
{
    public class ConfigurationChangedEventArgs : EventArgs
    {
        public ConfigurationChangedEventArgs(PacsConfiguration pacsConfiguration)
        {
            PacsConfiguration = pacsConfiguration;
        }

        public PacsConfiguration PacsConfiguration { get; set; }
    }
}
