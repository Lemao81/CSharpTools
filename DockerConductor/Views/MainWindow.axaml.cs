using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DockerConductor.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public StackPanel   ServiceSelectionContainer { get; set; }
        public ScrollViewer ScrollViewer              { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            ServiceSelectionContainer = this.Find<StackPanel>("serviceSelectionContainer");
            ScrollViewer              = this.Find<ScrollViewer>("ScrollViewer");
        }
    }
}
