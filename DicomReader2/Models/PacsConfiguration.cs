namespace DicomReader2.Models
{
    public class PacsConfiguration
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string CallingAet { get; set; }
        public string CalledAet { get; set; }
    }
}
