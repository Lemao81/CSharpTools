using System;

namespace DicomReader.WPF.Models
{
    public class ConfigurationChangedArgs : EventArgs
    {
        public ConfigurationChangedArgs(PacsConfiguration pacsConfiguration)
        {
            PacsConfiguration = pacsConfiguration;
        }

        public PacsConfiguration PacsConfiguration { get; set; }
    }
}
