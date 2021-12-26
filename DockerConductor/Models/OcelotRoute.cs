using System.Collections.Generic;
using System.Linq;

namespace DockerConductor.Models
{
    public class OcelotRoute
    {
        public string                         DownstreamPathTemplate { get; set; } = string.Empty;
        public string                         DownstreamScheme       { get; set; } = string.Empty;
        public IEnumerable<OcelotHostAndPort> DownstreamHostAndPorts { get; set; } = Enumerable.Empty<OcelotHostAndPort>();
        public string                         UpstreamPathTemplate   { get; set; } = string.Empty;
        public IEnumerable<string>            UpstreamHttpMethod     { get; set; } = Enumerable.Empty<string>();
        public string                         SwaggerKey             { get; set; } = string.Empty;
    }
}
