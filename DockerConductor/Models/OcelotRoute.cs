using System.Text.RegularExpressions;
using DockerConductor.Helpers;

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
        public string Path      { get; set; }

        public bool HasSwaggerKey => !string.IsNullOrWhiteSpace(Name);
        public bool IsWebSocket   => Schema == "ws";

        public void ReplaceHost(string host, string[] lines)
        {
            Host             = host;
            lines[HostIndex] = Regex.Replace(lines[HostIndex], RegexHelper.InBetweenRegexPattern(": \"", "\""), Host);
        }

        public void ReplacePort(int port, string[] lines)
        {
            Port             = port.ToString();
            lines[PortIndex] = Regex.Replace(lines[PortIndex], "\\d+", Port);
        }
    }
}
