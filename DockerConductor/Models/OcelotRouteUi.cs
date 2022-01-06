using Avalonia.Controls;
using DockerConductor.Constants;

namespace DockerConductor.Models
{
    public class OcelotRouteUi
    {
        public TextBlock? NameTextBlock  { get; set; }
        public CheckBox?  IsHostCheckBox { get; set; }
        public ComboBox?  PortSelection  { get; set; }

        public RadioButton? RadioButton80   { get; set; }
        public RadioButton? RadioButton5000 { get; set; }
        public RadioButton? RadioButton5001 { get; set; }
        public RadioButton? RadioButton5002 { get; set; }

        public string Name   => NameTextBlock?.Text ?? string.Empty;
        public bool   IsHost => IsHostCheckBox?.IsChecked == true;

        public int Port => PortSelection != null && int.TryParse(Consts.OcelotPortSelections[PortSelection.SelectedIndex], out _)
                               ? int.Parse(Consts.OcelotPortSelections[PortSelection.SelectedIndex])
                               : 5000;
    }
}
