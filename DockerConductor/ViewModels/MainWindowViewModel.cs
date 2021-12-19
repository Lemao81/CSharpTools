using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using DockerConductor.Helpers;
using DockerConductor.Models;
using DockerConductor.Views;
using Newtonsoft.Json;
using ReactiveUI;

namespace DockerConductor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _window;
        private          string     _dockerComposePath         = string.Empty;
        private          string     _dockerComposeOverridePath = string.Empty;
        private          string     _excludes                  = string.Empty;
        private          string     _firstBatch                = string.Empty;
        private          int        _firstBatchWait            = 10;
        private          string     _secondBatch               = string.Empty;
        private          int        _secondBatchWait           = 10;

        public MainWindowViewModel(Window window)
        {
            _window = window as MainWindow ?? throw new InvalidOperationException();
            InitializeCommands();
        }

        public IEnumerable<string> SelectedServiceNames => _window.ServiceSelectionCheckBoxes.Where(c => c.IsChecked == true)
                                                                  .Select(c => c.Content?.ToString())
                                                                  .Where(s => !string.IsNullOrWhiteSpace(s))!;

        public string DockerComposePath
        {
            get => _dockerComposePath;
            set => this.RaiseAndSetIfChanged(ref _dockerComposePath, value);
        }

        public string DockerComposeOverridePath
        {
            get => _dockerComposeOverridePath;
            set => this.RaiseAndSetIfChanged(ref _dockerComposeOverridePath, value);
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

        public string FirstBatchWait
        {
            get => _firstBatchWait.ToString();
            set => this.RaiseAndSetIfChanged(ref _firstBatchWait, int.Parse(value));
        }

        public string SecondBatch
        {
            get => _secondBatch;
            set => this.RaiseAndSetIfChanged(ref _secondBatch, value);
        }

        public string SecondBatchWait
        {
            get => _secondBatchWait.ToString();
            set => this.RaiseAndSetIfChanged(ref _secondBatchWait, int.Parse(value));
        }

        public ReactiveCommand<Unit, Task>? OpenDockerComposeFileSelection         { get; set; }
        public ReactiveCommand<Unit, Task>? OpenDockerComposeOverrideFileSelection { get; set; }
        public ReactiveCommand<Unit, Unit>? SaveConfiguration                      { get; set; }
        public ReactiveCommand<Unit, Unit>? DockerComposeUp                        { get; set; }
        public ReactiveCommand<Unit, Unit>? DockerComposeDown                      { get; set; }

        private void InitializeCommands()
        {
            OpenDockerComposeFileSelection = ReactiveCommand.Create(
                async () =>
                {
                    var files = await ShowYamlFileSelection("Select docker-compose.yml");
                    if (files.Length > 0)
                    {
                        DockerComposePath = files[0];

                        if (!string.IsNullOrWhiteSpace(DockerComposePath))
                        {
                            Helper.UpdateServiceCheckboxList(_window);
                        }
                    }
                }
            );

            OpenDockerComposeOverrideFileSelection = ReactiveCommand.Create(
                async () =>
                {
                    var files = await ShowYamlFileSelection("Select docker-compose.override.yml");
                    if (files.Length > 0)
                    {
                        DockerComposeOverridePath = files[0];
                    }
                }
            );

            SaveConfiguration = ReactiveCommand.Create(
                () =>
                {
                    var appConfig = new AppConfig
                    {
                        DockerComposePath         = DockerComposePath,
                        DockerComposeOverridePath = DockerComposeOverridePath,
                        Excludes                  = Excludes,
                        FirstBatch                = FirstBatch,
                        FirstBatchWait            = FirstBatchWait,
                        SecondBatch               = SecondBatch,
                        SecondBatchWait           = SecondBatchWait
                    };

                    File.WriteAllText(App.ConfigFileName, JsonConvert.SerializeObject(appConfig, Formatting.Indented));
                    if (!string.IsNullOrWhiteSpace(DockerComposePath))
                    {
                        Helper.UpdateServiceCheckboxList(_window);
                    }
                }
            );

            DockerComposeUp = ReactiveCommand.Create(
                () =>
                {
                    if (!SelectedServiceNames.Any()) return;

                    var basicCommand = Helper.ConcatCommand(
                        "docker-compose",
                        "-f",
                        $"\"{DockerComposePath}\"",
                        "-f",
                        $"\"{DockerComposeOverridePath}\"",
                        "up",
                        "-d",
                        "--no-deps"
                    );

                    var firstBatch  = Helper.FilterByCommaSeparated(SelectedServiceNames, FirstBatch).ToList();
                    var secondBatch = Helper.FilterByCommaSeparated(SelectedServiceNames, SecondBatch).ToList();
                    var rest        = SelectedServiceNames.Except(firstBatch).Except(secondBatch).ToList();

                    if (firstBatch.Any())
                    {
                        var firstBatchCommand = Helper.ConcatCommand(basicCommand, firstBatch);
                        Helper.ExecuteCliCommand(firstBatchCommand);

                        if (secondBatch.Any() || rest.Any())
                        {
                            Task.Delay(TimeSpan.FromSeconds(_firstBatchWait));
                        }
                    }

                    if (secondBatch.Any())
                    {
                        var secondBatchCommand = Helper.ConcatCommand(basicCommand, secondBatch);
                        Helper.ExecuteCliCommand(secondBatchCommand);

                        if (rest.Any())
                        {
                            Task.Delay(TimeSpan.FromSeconds(_secondBatchWait));
                        }
                    }

                    if (!rest.Any()) return;

                    var command = Helper.ConcatCommand(basicCommand, rest);
                    Helper.ExecuteCliCommand(command);
                }
            );

            DockerComposeDown = ReactiveCommand.Create(() => Helper.ExecuteCliCommand("docker-compose down"));
        }

        private async Task<string[]> ShowYamlFileSelection(string title)
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

            fileDialog.Title = title;
            var files = await fileDialog.ShowAsync(_window);
            return files;
        }
    }
}
