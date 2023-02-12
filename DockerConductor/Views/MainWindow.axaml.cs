using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DockerConductor.Models;
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

        public MainWindowViewModel       ViewModel                     => DataContext as MainWindowViewModel ?? throw new InvalidOperationException();
        public StackPanel?               ServiceSelectionContainer     { get; set; }
        public StackPanel?               BuildSelectionContainer       { get; set; }
        public IEnumerable<CheckBox>     ServiceSelectionCheckBoxes    => ServiceSelectionContainer?.Children.Cast<CheckBox>() ?? new List<CheckBox>();
        public IEnumerable<ToggleButton> BuildSelectionToggleButtons   => BuildSelectionContainer?.Children.Cast<ToggleButton>() ?? new List<ToggleButton>();
        public TextBlock?                ConsoleOutput                 { get; set; }
        public ScrollViewer?             ConsoleScrollViewer           { get; set; }
        public StackPanel?               OcelotItemContainer           { get; set; }
        public List<OcelotRouteUi>       OcelotRouteUis                { get; } = new();
        public StackPanel?               DockerContainerPanelContainer { get; set; }
        public TextBlock?                DockerApiStatus               { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            ServiceSelectionContainer     = this.Find<StackPanel>("ServiceSelectionContainer");
            BuildSelectionContainer       = this.Find<StackPanel>("BuildSelectionContainer");
            ConsoleOutput                 = this.Find<TextBlock>("ConsoleOutput");
            ConsoleScrollViewer           = this.Find<ScrollViewer>("ConsoleScrollViewer");
            OcelotItemContainer           = this.Find<StackPanel>("OcelotItemContainer");
            DockerContainerPanelContainer = this.Find<StackPanel>("DockerContainerPanelContainer");
            DockerApiStatus               = this.Find<TextBlock>("DockerApiStatus");
        }

        private void ContainerTab_OnTapped(object? sender, RoutedEventArgs e) => Dispatcher.UIThread.InvokeAsync(() => ViewModel.OnContainerTabTappedAsync());
    }
}
