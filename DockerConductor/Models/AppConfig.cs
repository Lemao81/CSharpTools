﻿namespace DockerConductor.Models
{
    public class AppConfig
    {
        public string DockerComposePath { get; set; } = string.Empty;
        public string Excludes          { get; set; } = string.Empty;
        public string FirstBatch        { get; set; } = string.Empty;
        public string SecondBatch       { get; set; } = string.Empty;
    }
}
