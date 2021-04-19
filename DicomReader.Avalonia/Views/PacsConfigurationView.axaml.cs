using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DicomReader.Avalonia.ViewModels;

namespace DicomReader.Avalonia.Views
{
    public class PacsConfigurationView : UserControl
    {
        public PacsConfigurationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
