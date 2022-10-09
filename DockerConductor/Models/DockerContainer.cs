namespace DockerConductor.Models
{
    public class DockerContainer
    {
        public string Id    { get; set; } = string.Empty;
        public string Name  { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}
