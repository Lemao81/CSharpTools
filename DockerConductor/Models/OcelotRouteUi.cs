using System;
using Avalonia.Controls;

namespace DockerConductor.Models
{
    public class OcelotRouteUi
    {
        public TextBlock?   NameTextBlock   { get; set; }
        public RadioButton? RadioButton80   { get; set; }
        public RadioButton? RadioButton5000 { get; set; }
        public RadioButton? RadioButton5001 { get; set; }
        public RadioButton? RadioButton5002 { get; set; }
        public string       OrigHost        { get; set; } = string.Empty;

        public string Name           => NameTextBlock?.Text ?? string.Empty;
        public bool   IsInternalHost => RadioButton80?.IsChecked != true;

        public int Port
        {
            get
            {
                if (RadioButton80?.IsChecked == true) return 80;
                if (RadioButton5000?.IsChecked == true) return 5000;
                if (RadioButton5001?.IsChecked == true) return 5001;
                if (RadioButton5002?.IsChecked == true) return 5002;

                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
