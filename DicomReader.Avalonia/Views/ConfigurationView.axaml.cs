using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DicomReader.Avalonia.Views
{
    public class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
