using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DicomReader.Avalonia.Views
{
    public class DicomQueryUserControl : UserControl
    {
        public DicomQueryUserControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}