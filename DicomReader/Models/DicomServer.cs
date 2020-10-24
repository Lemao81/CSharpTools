namespace DicomReader.Models
{
    public class DicomServer
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string CallingApplicationEntity { get; set; }
        public string CalledApplicationEntity { get; set; }
    }
}