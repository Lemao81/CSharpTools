namespace DockerConductor.Models
{
    public class AppConfig
    {
        public string DockerComposePath         { get; set; } = string.Empty;
        public string DockerComposeOverridePath { get; set; } = string.Empty;
        public string Excludes                  { get; set; } = string.Empty;
        public string ThirdParties              { get; set; } = string.Empty;
        public string Usuals                    { get; set; } = string.Empty;
        public string FirstBatch                { get; set; } = string.Empty;
        public string FirstBatchWait            { get; set; } = "10";
        public string SecondBatch               { get; set; } = string.Empty;
        public string SecondBatchWait           { get; set; } = "10";
    }
}
