using System;

namespace DicomReader.WPF.Models
{
    public class ConfigurationChangedArgs : EventArgs
    {
        public PacsConfiguration PacsConfiguration { get; set; }
    }
}
