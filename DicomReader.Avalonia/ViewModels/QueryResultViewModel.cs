using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            ConfigureCopyButton();
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
                if (value == default) return;

                this.RaiseAndSetIfChanged(ref _resultTab, value);
            }
        }

        public bool IsPagedResult { get; set; } = true;

        public ObservableCollection<List<DicomResult>> Results { get; } = new();

        public ObservableCollection<DicomResult> FlattenedResults { get; } = new();

        public ReactiveCommand<Unit, Unit>? Copy { get; protected set; }

        public ReactiveCommand<Unit, Unit>? RequestNextPage { get; protected set; }

        private void ConfigureCopyButton()
        {
            var enableObservable = this.WhenAnyValue(vm => vm.ResultTab, tab => tab == ResultTab.Json || tab == ResultTab.Values);
            Copy = ReactiveCommand.CreateFromTask(async () =>
            {
                switch (ResultTab)
                {
                    case ResultTab.Json:
                        await Application.Current.Clipboard.SetTextAsync(Json);
                        break;
                    case ResultTab.Values:
                        var content = string.Join("\n", FlattenedResults.Select(r => r.StringValue));
                        await Application.Current.Clipboard.SetTextAsync(content);
                        break;
                }
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
