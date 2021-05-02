using System.Collections.ObjectModel;
using DicomReader.Avalonia.Models;
using ReactiveUI;

namespace DicomReader.Avalonia.ViewModels
{
    public class QueryResultViewModel : ViewModelBase
    {
        private string _json;

        public string Json
        {
            get => _json;
            set => this.RaiseAndSetIfChanged(ref _json, value);
        }

        public ObservableCollection<DicomResult> Results { get; } = new();
    }
}
