using Avalonia.Controls;
using DockerConductor.Helpers;
using DockerConductor.Models;
using DockerConductor.Views;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using DockerConductor.Constants;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace DockerConductor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _window;
        private          string     _dockerComposePath         = string.Empty;
        private          string     _dockerComposeOverridePath = string.Empty;
        private          string     _excludes                  = string.Empty;
        private          string     _thirdParties              = string.Empty;
        private          string     _usuals                    = string.Empty;
        private          string     _firstBatch                = string.Empty;
        private          int        _firstBatchWait;
        private          string     _secondBatch = string.Empty;
        private          int        _secondBatchWait;
        private          string     _dbVolume = string.Empty;

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(Window window)
        {
            _window = window as MainWindow ?? throw new InvalidOperationException();
            InitializeCommands();
            _window.Closing += (_, _) =>
                               {
                                   LastSelected = SelectedServiceNames;
                                   WriteConfig();
                               };
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

        public string ThirdParties
        {
            get => _thirdParties;
            set => this.RaiseAndSetIfChanged(ref _thirdParties, value);
        }

        public string Usuals
        {
            get => _usuals;
            set => this.RaiseAndSetIfChanged(ref _usuals, value);
        }

        public string FirstBatch
        {
            get => _firstBatch;
            set => this.RaiseAndSetIfChanged(ref _firstBatch, value);
        }

        public string FirstBatchWait
        {
            get => _firstBatchWait.ToString();
            set => this.RaiseAndSetIfChanged(ref _firstBatchWait, int.TryParse(value, out _) ? int.Parse(value) : 0);
        }

        public string SecondBatch
        {
            get => _secondBatch;
            set => this.RaiseAndSetIfChanged(ref _secondBatch, value);
        }

        public string SecondBatchWait
        {
            get => _secondBatchWait.ToString();
            set => this.RaiseAndSetIfChanged(ref _secondBatchWait, int.TryParse(value, out _) ? int.Parse(value) : 0);
        }

        public string DbVolume
        {
            get => _dbVolume;
            set => this.RaiseAndSetIfChanged(ref _dbVolume, value);
        }

        public IEnumerable<string> LastSelected { get; set; } = Enumerable.Empty<string>();

        public ReactiveCommand<Unit, Task>? OpenDockerComposeFileSelection         { get; set; }
        public ReactiveCommand<Unit, Task>? OpenDockerComposeOverrideFileSelection { get; set; }
        public ReactiveCommand<Unit, Unit>? SaveConfiguration                      { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeUp                        { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeDown                      { get; set; }
        public ReactiveCommand<Unit, Task>? DockerPs                               { get; set; }
        public ReactiveCommand<Unit, Task>? DockerDbResetPrune                     { get; set; }
        public ReactiveCommand<Unit, Task>? DockerBuildConfirmation                { get; set; }
        public ReactiveCommand<Unit, Unit>? DeselectAll                            { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectThirdParties                     { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectUsuals                           { get; set; }

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
                    WriteConfig();
                    if (!string.IsNullOrWhiteSpace(DockerComposePath))
                    {
                        Helper.UpdateServiceCheckboxList(_window);
                    }
                }
            );

            DockerComposeUp = ReactiveCommand.Create(
                async () =>
                {
                    if (!SelectedServiceNames.Any()) return;

                    var basicCommand = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(DockerComposePath, DockerComposeOverridePath),
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
                        await Helper.ExecuteCliCommand(firstBatchCommand, _window);

                        if (secondBatch.Any() || rest.Any())
                        {
                            await Task.Delay(TimeSpan.FromSeconds(_firstBatchWait));
                        }
                    }

                    if (secondBatch.Any())
                    {
                        var secondBatchCommand = Helper.ConcatCommand(basicCommand, secondBatch);
                        await Helper.ExecuteCliCommand(secondBatchCommand, _window);

                        if (rest.Any())
                        {
                            await Task.Delay(TimeSpan.FromSeconds(_secondBatchWait));
                        }
                    }

                    if (!rest.Any()) return;

                    var command = Helper.ConcatCommand(basicCommand, rest);
                    await Helper.ExecuteCliCommand(command, _window);
                }
            );

            DockerComposeDown = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(DockerComposePath, DockerComposeOverridePath),
                        "down"
                    );

                    await Helper.ExecuteCliCommand(command, _window);
                }
            );

            DockerPs = ReactiveCommand.Create(async () => await Helper.ExecuteCliCommand(Helper.ConcatCommand("docker", "ps"), _window));

            DockerDbResetPrune = ReactiveCommand.Create(
                async () =>
                {
                    if (string.IsNullOrWhiteSpace(DbVolume)) return;

                    var command = $"docker volume rm {DbVolume} && docker system prune -f";
                    await Helper.ExecuteCliCommand(command, _window);
                }
            );

            DockerBuildConfirmation = ReactiveCommand.Create(
                async () =>
                {
                    var result = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(
                                                     new MessageBoxStandardParams
                                                     {
                                                         ContentTitle          = "Build",
                                                         ContentMessage        = "Sure you want to build?",
                                                         ButtonDefinitions     = ButtonEnum.YesNo,
                                                         WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                                         Icon                  = Icon.Warning
                                                     }
                                                 )
                                                 .Show();

                    if (result == ButtonResult.Yes)
                    {
                        var basicCommand = Helper.ConcatCommand(
                            Consts.DockerCompose,
                            Helper.ConcatFilePathArguments(DockerComposePath, DockerComposeOverridePath),
                            "build"
                        );

                        var command = Helper.ConcatCommand(basicCommand, SelectedServiceNames);
                        await Helper.ExecuteCliCommand(command, _window);
                    }
                }
            );

            DeselectAll = ReactiveCommand.Create(
                () =>
                {
                    foreach (var checkBox in _window.ServiceSelectionCheckBoxes)
                    {
                        checkBox.IsChecked = false;
                    }
                }
            );

            SelectThirdParties = ReactiveCommand.Create(
                () => Helper.SelectMatchingContents(_window.ServiceSelectionCheckBoxes, Helper.SplitCommaSeparated(ThirdParties))
            );

            SelectUsuals = ReactiveCommand.Create(() => Helper.SelectMatchingContents(_window.ServiceSelectionCheckBoxes, Helper.SplitCommaSeparated(Usuals)));
        }

        private void WriteConfig()
        {
            var appConfig = new AppConfig();
            appConfig.MapFrom(this);

            File.WriteAllText(App.ConfigFileName, JsonConvert.SerializeObject(appConfig, Formatting.Indented));
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
