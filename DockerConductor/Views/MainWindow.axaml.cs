using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DockerConductor.ViewModels;

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

        public MainWindowViewModel   ViewModel                  => DataContext as MainWindowViewModel ?? throw new InvalidOperationException();
        public StackPanel?           ServiceSelectionContainer  { get; set; }
        public IEnumerable<CheckBox> ServiceSelectionCheckBoxes => ServiceSelectionContainer?.Children.Cast<CheckBox>() ?? new List<CheckBox>();
        public TextBlock?            ConsoleOutput              { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            ServiceSelectionContainer = this.Find<StackPanel>("ServiceSelectionContainer");
            ConsoleOutput             = this.Find<TextBlock>("ConsoleOutput");
        }
    }
}
