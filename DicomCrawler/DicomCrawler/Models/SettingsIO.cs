namespace DicomCrawler.Models
{
    public class SettingsIO
    {
        public string PacsHost { get; set; }
        public int PacsPort { get; set; }
        public string PacsCallingAet { get; set; }
        public string PacsCalledAet { get; set; }

        public SettingsIO()
        {
        }

        public SettingsIO(Settings settings)
        {
            PacsHost = settings.PacsHost;
            PacsPort = settings.PacsPort;
            PacsCallingAet = settings.PacsCallingAet;
            PacsCalledAet = settings.PacsCalledAet;
        }
    }
}
