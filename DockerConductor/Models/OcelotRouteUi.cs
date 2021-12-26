using Avalonia.Controls;

namespace DockerConductor.Models
{
    public class OcelotRouteUi
    {
        public TextBlock Name   { get; set; }
        public CheckBox  IsHost { get; set; }
        public ComboBox  PortSelection   { get; set; }
    }
}
