using System;
using DicomReader.WPF.Models;
using Prism.Mvvm;

namespace DicomReader.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Dicom Reader";

        public static event EventHandler<LogEntryEmittedArgs> LogEntryEmitted;

        public MainWindowViewModel()
        {
            ConfigurationTabUserControlViewModel.ConfigurationChanged += (s, e) => SelectedConfiguration = e.PacsConfiguration;
        }

        public static PacsConfiguration SelectedConfiguration { get; set; }

        public static void EmitLogEntry(string logEntry) => LogEntryEmitted?.Invoke(null, new LogEntryEmittedArgs(logEntry));

        #region bound properties
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        #endregion
    }
}
