using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Common.Extensions;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;
using DynamicData;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace DicomReader.Avalonia.ViewModels
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        private string _host = string.Empty;
        private string _port = string.Empty;
        private string _callingAe = string.Empty;
        private string _calledAe = string.Empty;
        private PacsConfigurationViewMode _viewMode;
        private PacsConfiguration? _selectedConfiguration;
        private PacsConfiguration? _selectedConvifugrationBeforeEditing;
        private readonly List<IObserver<ConfigurationChangedData>> _configurationChangedStreamObservers = new();
        private readonly IEnumerable<string> _viewModeDependantProperties = new[] { nameof(IsSelectedMode), nameof(IsEditingMode) };
        private OutputFormat _outputFormat = OutputFormat.JsonSerialized;

        public ConfigurationViewModel()
        {
            ConfigureAddPacsConfigurationButton();
            ConfigureSavePacsConfigurationButton();
            ConfigureCancelEditingButton();
            ConfigureConfigurationChangedStream();
        }

        public void Initialize(AppConfig appConfig)
        {
            PacsConfigurations.Clear();
            PacsConfigurations.AddRange(appConfig.PacsConfigurations);
            if (!appConfig.LastLoadedPacsConfiguration.IsNullOrEmpty())
            {
                SelectedConfiguration = PacsConfigurations.Single(config => config.Name == appConfig.LastLoadedPacsConfiguration);
            }

            OutputFormat = appConfig.OutputFormat;
        }

        public ObservableCollection<PacsConfiguration> PacsConfigurations { get; } = new();

        public PacsConfiguration? SelectedConfiguration
        {
            get => _selectedConfiguration;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedConfiguration, value);
                if (value == null) return;

                Name = value.Name;
                Host = value.Host;
                Port = value.Port.ToString();
                CallingAe = value.CallingAe;
                CalledAe = value.CalledAe;
                ViewMode = PacsConfigurationViewMode.Selected;
                EmitConfigurationChanged(new ConfigurationChangedData(value.Name));
            }
        }

        public PacsConfigurationViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _viewMode, value);
                foreach (var property in _viewModeDependantProperties) this.RaisePropertyChanged(property);
            }
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Host
        {
            get => _host;
            set => this.RaiseAndSetIfChanged(ref _host, value);
        }

        public string Port
        {
            get => _port;
            set => this.RaiseAndSetIfChanged(ref _port, value);
        }

        public string CallingAe
        {
            get => _callingAe;
            set => this.RaiseAndSetIfChanged(ref _callingAe, value);
        }

        public string CalledAe
        {
            get => _calledAe;
            set => this.RaiseAndSetIfChanged(ref _calledAe, value);
        }

        public OutputFormat OutputFormat
        {
            get => _outputFormat;
            set
            {
                if (value == default) return;

                this.RaiseAndSetIfChanged(ref _outputFormat, value);
                EmitConfigurationChanged(new ConfigurationChangedData(value));
            }
        }

        public bool IsSelectedMode => ViewMode == PacsConfigurationViewMode.Selected;
        public bool IsEditingMode => ViewMode is PacsConfigurationViewMode.Add or PacsConfigurationViewMode.Edit;
        public IObservable<ConfigurationChangedData> ConfigurationChangedStream { get; protected set; }

        public ReactiveCommand<Unit, Unit>? AddPacsConfiguration { get; protected set; }
        public ReactiveCommand<Unit, PacsConfiguration>? SavePacsConfiguration { get; protected set; }
        public ReactiveCommand<Unit, Unit>? CancelEditing { get; protected set; }

        private void EmitConfigurationChanged(ConfigurationChangedData changedData) =>
            _configurationChangedStreamObservers.ForEach(obs => obs.OnNext(changedData));

        private void ConfigureAddPacsConfigurationButton()
        {
            var enabledObservable = this.WhenAnyValue(v => v.ViewMode, m => m != PacsConfigurationViewMode.Add);
            AddPacsConfiguration = ReactiveCommand.Create(() =>
            {
                EmptyInputFields();
                ViewMode = PacsConfigurationViewMode.Add;
                SelectedConfiguration = null;
            }, enabledObservable);
        }

        private void ConfigureSavePacsConfigurationButton()
        {
            var enabledObservable =
                this.WhenAnyValue(v => v.ViewMode, m => m == PacsConfigurationViewMode.Add || m == PacsConfigurationViewMode.Edit);
            SavePacsConfiguration = ReactiveCommand.Create(() =>
            {
                var error = ValidatePacsConfigurationInput();
                if (!error.IsNullOrEmpty())
                {
                    ShowInvalidInputMessage(error);
                    throw new InvalidOperationException(error);
                }
                ViewMode = PacsConfigurationViewMode.None;

                return new PacsConfiguration(this);
            }, enabledObservable);
        }

        private void ConfigureCancelEditingButton()
        {
            var enabledObservable = this.WhenAnyValue(v => v.IsEditingMode);
            CancelEditing = ReactiveCommand.Create(() =>
            {
                if (_selectedConvifugrationBeforeEditing != null)
                {
                    SelectedConfiguration = _selectedConvifugrationBeforeEditing;
                    ViewMode = PacsConfigurationViewMode.Selected;

                    return;
                }

                EmptyInputFields();
                ViewMode = PacsConfigurationViewMode.None;
            }, enabledObservable);
        }

        private void ConfigureConfigurationChangedStream()
        {
            ConfigurationChangedStream = Observable.Create<ConfigurationChangedData>(observer =>
            {
                _configurationChangedStreamObservers.Add(observer);

                return new ObserverDisposable<ConfigurationChangedData>(observer, _configurationChangedStreamObservers);
            });
        }

        private string ValidatePacsConfigurationInput()
        {
            if (Name.IsNullOrEmpty()) return "Name mustn't be empty";

            if (Host.IsNullOrEmpty()) return "Host mustn't be empty";

            if (Port.IsNullOrEmpty()) return "Port mustn't be empty";

            if (!int.TryParse(Port, out var port) || port < 0) return "Port must be an integer greater than zero";

            return string.Empty;
        }

        private static void ShowInvalidInputMessage(string error) =>
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                Icon = Icon.Error,
                ContentTitle = "Invalid input",
                ContentMessage = error,
                ButtonDefinitions = ButtonEnum.Ok,
                Style = Style.Windows
            }).Show();

        private void EmptyInputFields()
        {
            Name = string.Empty;
            Host = string.Empty;
            Port = string.Empty;
            CallingAe = string.Empty;
            CalledAe = string.Empty;
        }
    }
}
