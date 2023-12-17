using System.Collections.Generic;

namespace DockerConductor.Models
{
    public class TraefikServiceLoadBalancer
    {
        public IEnumerable<TraefikServiceServer> Servers { get; set; }
    }
}
