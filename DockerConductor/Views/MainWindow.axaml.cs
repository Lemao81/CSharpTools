using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DockerConductor.CustomViews;
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
        public ListBox?                  ConsoleOutput                 { get; set; }
        public StackPanel?               OcelotItemContainer           { get; set; }
        public StackPanel?               TraefikItemContainer          { get; set; }
        public List<RouteUi>             OcelotRouteUis                { get; } = new();
        public List<RouteUi>             TraefikRouteUis               { get; } = new();
        public StackPanel?               DockerContainerPanelContainer { get; set; }
        public TextBlock?                DockerApiStatus               { get; set; }
        public TabControl?               TabControl                    { get; set; }
        public BusyBeacon?               PanelBusyBeacon               { get; set; }
        public BusyBeacon?               BuildBusyBeacon               { get; set; }
        public BusyBeacon?               OcelotBusyBeacon              { get; set; }
        public BusyBeacon?               TraefikBusyBeacon             { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            ServiceSelectionContainer     = this.Find<StackPanel>("ServiceSelectionContainer");
            BuildSelectionContainer       = this.Find<StackPanel>("BuildSelectionContainer");
            ConsoleOutput                 = this.Find<ListBox>("ConsoleOutput");
            OcelotItemContainer           = this.Find<StackPanel>("OcelotItemContainer");
            TraefikItemContainer          = this.Find<StackPanel>("TraefikItemContainer");
            DockerContainerPanelContainer = this.Find<StackPanel>("DockerContainerPanelContainer");
            DockerApiStatus               = this.Find<TextBlock>("DockerApiStatus");
            TabControl                    = this.Find<TabControl>("TabControl");
            PanelBusyBeacon               = this.Find<BusyBeacon>("PanelBusyBeacon");
            BuildBusyBeacon               = this.Find<BusyBeacon>("BuildBusyBeacon");
            OcelotBusyBeacon              = this.Find<BusyBeacon>("OcelotBusyBeacon");
            TraefikBusyBeacon             = this.Find<BusyBeacon>("TraefikBusyBeacon");
        }

        private void ContainerTab_OnTapped(object? _, RoutedEventArgs __) => Dispatcher.UIThread.InvokeAsync(() => ViewModel.OnContainerTabTappedAsync());
    }
}
