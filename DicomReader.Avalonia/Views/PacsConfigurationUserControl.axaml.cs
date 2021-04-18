using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DicomReader.Avalonia.Views
{
    public class PacsConfigurationUserControl : UserControl
    {
        public PacsConfigurationUserControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}