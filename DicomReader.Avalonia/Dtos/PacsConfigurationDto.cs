namespace DicomReader.Avalonia.Dtos
{
    public class PacsConfigurationDto
    {
        public string? Name { get; set; }
        public string? Host { get; set; }
        public string? Port { get; set; }
        public string? CallingAe { get; set; }
        public string? CalledAe { get; set; }
    }
}
