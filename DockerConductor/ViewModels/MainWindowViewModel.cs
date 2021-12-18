using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using DockerConductor.Models;
using Newtonsoft.Json;
using ReactiveUI;

namespace DockerConductor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Window _window;
        private          string _dockerComposePath = string.Empty;
        private          string _excludes          = string.Empty;
        private          string _firstBatch        = string.Empty;
        private          string _secondBatch       = string.Empty;

        public MainWindowViewModel(Window window)
        {
            _window = window;
            InitializeCommands();
        }

        public string DockerComposePath
        {
            get => _dockerComposePath;
            set => this.RaiseAndSetIfChanged(ref _dockerComposePath, value);
        }

        public string Excludes
        {
            get => _excludes;
            set => this.RaiseAndSetIfChanged(ref _excludes, value);
        }

        public string FirstBatch
        {
            get => _firstBatch;
            set => this.RaiseAndSetIfChanged(ref _firstBatch, value);
        }

        public string SecondBatch
        {
            get => _secondBatch;
            set => this.RaiseAndSetIfChanged(ref _secondBatch, value);
        }

        public ReactiveCommand<Unit, Task>? OpenYmlFileSelection { get; set; }
        public ReactiveCommand<Unit, Unit>? SaveConfiguration    { get; set; }

        private void InitializeCommands()
        {
            OpenYmlFileSelection = ReactiveCommand.Create(
                async () =>
                {
                    var fileDialog = new OpenFileDialog { AllowMultiple = false };

                    fileDialog.Filters.Add(
                        new FileDialogFilter
                        {
                            Name = "yaml",
                            Extensions = new List<string>
                            {
                                "yml",
                                "yaml"
                            }
                        }
                    );

                    fileDialog.Title = "Select docker-compose.yml";
                    var files = await fileDialog.ShowAsync(_window);
                    if (files.Length > 0)
                    {
                        DockerComposePath = files[0];

                        if (!string.IsNullOrWhiteSpace(DockerComposePath))
                        {
                            App.AddServiceSelections(DockerComposePath, _window);
                        }
                    }
                }
            );

            SaveConfiguration = ReactiveCommand.Create(
                () =>
                {
                    var appConfig = new AppConfig
                    {
                        DockerComposePath = DockerComposePath,
                        Excludes          = Excludes,
                        FirstBatch        = FirstBatch,
                        SecondBatch       = SecondBatch
                    };

                    File.WriteAllText(App.ConfigFileName, JsonConvert.SerializeObject(appConfig, Formatting.Indented));
                }
            );
        }
    }
}
