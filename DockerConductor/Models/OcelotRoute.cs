namespace DockerConductor.Models
{
    public class OcelotRoute
    {
        public string Name      { get; set; }
        public string Host      { get; set; }
        public int    HostIndex { get; set; }
        public string OrigHost  { get; set; }
        public string Port      { get; set; }
        public int    PortIndex { get; set; }
        public string Schema    { get; set; }

        public bool HasSwaggerKey => !string.IsNullOrWhiteSpace(Name);
        public bool IsWebSocket   => Schema == "ws";
    }
}
