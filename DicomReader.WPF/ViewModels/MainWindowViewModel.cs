using System;
using DicomReader.WPF.Models;
using DicomReader.WPF.Models.Event;
using Prism.Mvvm;

namespace DicomReader.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Dicom Reader";
        private int _selectedIndex;

        public static event EventHandler<LogEntryEmittedEventArgs> LogEntryEmitted;
        public static event EventHandler<SwitchTabEventArgs> SwitchTabEmitted;

        public MainWindowViewModel()
        {
            ConfigurationTabUserControlViewModel.ConfigurationChanged += (s, e) => SelectedConfiguration = e.PacsConfiguration;
            SwitchTabEmitted += (s, e) => SelectedIndex = e.SelectedIndex;
        }

        public static PacsConfiguration SelectedConfiguration { get; set; }

        public static void EmitLogEntry(string logEntry) => LogEntryEmitted?.Invoke(null, new LogEntryEmittedEventArgs(logEntry));

        public static void EmitSwitchTab(int index) => SwitchTabEmitted?.Invoke(null, new SwitchTabEventArgs(index));

        #region bound properties
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }
        #endregion
    }
}
