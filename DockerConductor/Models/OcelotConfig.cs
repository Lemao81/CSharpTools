using System.Collections.Generic;
using System.Linq;

namespace DockerConductor.Models
{
    public class OcelotConfig
    {
        public IEnumerable<OcelotRoute> Routes              { get; set; } = Enumerable.Empty<OcelotRoute>();
        public object                   GlobalConfiguration { get; set; } = new object();
        public object                   Logging             { get; set; } = new object();
        public string                   AllowedHosts        { get; set; } = string.Empty;
    }
}
