using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;
using ReactiveUI;

namespace DicomReader.Avalonia.ViewModels
{
    public class QueryResultViewModel : ViewModelBase
    {
        private string _json;
        private ResultTab _resultTab;

        public QueryResultViewModel()
        {
            ConfigureCopyJsonButton();
            ConfigureRequestNextPageButton();
        }

        public event EventHandler NextPageRequested;

        public string Json
        {
            get => _json;
            set => this.RaiseAndSetIfChanged(ref _json, value);
        }

        public ResultTab ResultTab
        {
            get => _resultTab;
            set
            {
                this.RaiseAndSetIfChanged(ref _resultTab, value);
                this.RaisePropertyChanged(nameof(IsJsonSelected));
                this.RaisePropertyChanged(nameof(IsTableSelected));
            }
        }

        public bool IsPagedResult { get; set; } = true;

        public bool IsJsonSelected
        {
            get => ResultTab == ResultTab.Json;
            set => ResultTab = ResultTab.Json;
        }

        public bool IsTableSelected
        {
            get => ResultTab == ResultTab.Table;
            set => ResultTab = ResultTab.Table;
        }

        public ObservableCollection<DicomResult> Results { get; } = new();

        public ReactiveCommand<Unit, Unit>? CopyJson { get; protected set; }

        public ReactiveCommand<Unit, Unit>? RequestNextPage { get; protected set; }

        private void ConfigureCopyJsonButton()
        {
            var enableObservable = this.WhenAnyValue(vm => vm.IsJsonSelected);
            CopyJson = ReactiveCommand.CreateFromTask(async () =>
            {
                await Application.Current.Clipboard.SetTextAsync(Json);
            }, enableObservable);
        }

        private void ConfigureRequestNextPageButton()
        {
            var enableObservable = this.WhenAnyValue(vm => vm.IsPagedResult);
            RequestNextPage = ReactiveCommand.Create(() =>
            {
                NextPageRequested.Invoke(this, EventArgs.Empty);
            }, enableObservable);
        }
    }
}
