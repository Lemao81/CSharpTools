using System.Collections.Generic;

namespace DockerConductor.Models
{
    public class TraefikServices
    {
        public IDictionary<string, TraefikService>? Services { get; set; }
    }
}
