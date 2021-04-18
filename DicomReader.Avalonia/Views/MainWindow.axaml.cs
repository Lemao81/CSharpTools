using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DicomReader.Avalonia.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            System.Diagnostics.Trace.TraceInformation("Started");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
