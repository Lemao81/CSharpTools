﻿using Avalonia.Controls;
using DockerConductor.Helpers;
using DockerConductor.Models;
using DockerConductor.Views;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Layout;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerConductor.Commands;
using DockerConductor.Constants;
using DockerConductor.Extensions;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json.Linq;

namespace DockerConductor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow                   _window;
        private          string                       _excludes     = string.Empty;
        private          string                       _thirdParties = string.Empty;
        private          string                       _usersSetup   = string.Empty;
        private          string                       _startUsuals  = string.Empty;
        private          string                       _buildUsuals  = string.Empty;
        private          string                       _firstBatch   = string.Empty;
        private          int                          _firstBatchWait;
        private          string                       _secondBatch = string.Empty;
        private          int                          _secondBatchWait;
        private          string                       _excludesStop            = string.Empty;
        private          string                       _dbVolume                = string.Empty;
        public           string                       _devServerIp             = string.Empty;
        private          string                       _ocelotConfigurationPath = string.Empty;
        private          string                       _executedCommand         = string.Empty;
        private          string                       _frontendRepoPath        = string.Empty;
        private          string                       _backendRepoPath         = string.Empty;
        private          DockerClient                 _dockerClient;
        private          Encoding                     _encoding            = new UTF8Encoding(false);
        private          ObservableCollection<string> _consoleOutputItems  = new();
        private          string                       _traefikServicesPath = string.Empty;

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

            _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }

        public IEnumerable<string> SelectedServiceNames => _window.ServiceSelectionCheckBoxes.Where(c => c.IsChecked == true)
                                                                  .Select(c => c.Content?.ToString())
                                                                  .Where(s => !string.IsNullOrWhiteSpace(s))!;

        public IEnumerable<string> SelectedBuildNames => _window.BuildSelectionToggleButtons.Where(c => c.IsChecked == true)
                                                                .Select(c => c.Content?.ToString())
                                                                .Where(s => !string.IsNullOrWhiteSpace(s))!;

        public IEnumerable<string>  LastSelected            { get; set; } = Enumerable.Empty<string>();
        public string[]             OcelotConfigLines       { get; set; } = Array.Empty<string>();
        public List<OcelotRoute>    OcelotRoutes            { get; }      = new();
        public TraefikServicesHttp? TraefikServicesHttp     { get; set; }
        public string               TraefikServicesOrigText { get; set; }

        public string BackendRepoPath
        {
            get => _backendRepoPath;
            set => this.RaiseAndSetIfChanged(ref _backendRepoPath, value);
        }

        public string BackendDockerComposePath => Path.Join(BackendRepoPath, "docker-compose.yml");

        public string BackendDockerComposeOverridePath => Path.Join(BackendRepoPath, "docker-compose.override.yml");

        public string FrontendRepoPath
        {
            get => _frontendRepoPath;
            set => this.RaiseAndSetIfChanged(ref _frontendRepoPath, value);
        }

        public string FrontendDockerComposePath => Path.Join(FrontendRepoPath, "docker-compose.yml");

        public string FrontendDockerComposeOverridePath => Path.Join(FrontendRepoPath, "docker-compose.override.yml");

        public string OcelotConfigurationPath
        {
            get => _ocelotConfigurationPath;
            set => this.RaiseAndSetIfChanged(ref _ocelotConfigurationPath, value);
        }

        public string TraefikServicesPath
        {
            get => _traefikServicesPath;
            set => this.RaiseAndSetIfChanged(ref _traefikServicesPath, value);
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

        public string UsersSetup
        {
            get => _usersSetup;
            set => this.RaiseAndSetIfChanged(ref _usersSetup, value);
        }

        public string StartUsuals
        {
            get => _startUsuals;
            set => this.RaiseAndSetIfChanged(ref _startUsuals, value);
        }

        public string BuildUsuals
        {
            get => _buildUsuals;
            set => this.RaiseAndSetIfChanged(ref _buildUsuals, value);
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

        public string ExcludesStop
        {
            get => _excludesStop;
            set => this.RaiseAndSetIfChanged(ref _excludesStop, value);
        }

        public string DbVolume
        {
            get => _dbVolume;
            set => this.RaiseAndSetIfChanged(ref _dbVolume, value);
        }

        public string DevServerIp
        {
            get => _devServerIp;
            set => this.RaiseAndSetIfChanged(ref _devServerIp, value);
        }

        public string ExecutedCommand
        {
            get => _executedCommand;
            set => this.RaiseAndSetIfChanged(ref _executedCommand, value);
        }

        public ObservableCollection<string> ConsoleOutputItems
        {
            get => _consoleOutputItems;
            set => this.RaiseAndSetIfChanged(ref _consoleOutputItems, value);
        }

        public ReactiveCommand<Unit, Task>? OpenBackendFolderSelection       { get; set; }
        public ReactiveCommand<Unit, Task>? OpenFrontendFolderSelection      { get; set; }
        public ReactiveCommand<Unit, Task>? OcelotConfigurationFileSelection { get; set; }
        public ReactiveCommand<Unit, Unit>? SaveConfiguration                { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeUpDelayed           { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeUp                  { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeStop                { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeStart               { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeDown                { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeBuildOcelot         { get; set; }
        public ReactiveCommand<Unit, Task>? DockerPs                         { get; set; }
        public ReactiveCommand<Unit, Task>? DockerPsExited                   { get; set; }
        public ReactiveCommand<Unit, Task>? DockerDbResetPrune               { get; set; }
        public ReactiveCommand<Unit, Task>? DockerBuildAllConfirmation       { get; set; }
        public ReactiveCommand<Unit, Task>? FrontendDockerComposeUp          { get; set; }
        public ReactiveCommand<Unit, Task>? FrontendDockerComposeDown        { get; set; }
        public ReactiveCommand<Unit, Task>? BackendSelectedBuild             { get; set; }
        public ReactiveCommand<Unit, Task>? FrontendBuild                    { get; set; }
        public ReactiveCommand<Unit, Unit>? FrontendAdjustDevConfigLocalhost { get; set; }
        public ReactiveCommand<Unit, Unit>? FrontendAdjustDevConfigDevServer { get; set; }
        public ReactiveCommand<Unit, Unit>? FrontendRemoveModules            { get; set; }
        public ReactiveCommand<Unit, Unit>? DeselectAll                      { get; set; }
        public ReactiveCommand<Unit, Unit>? BuildSelectAll                   { get; set; }
        public ReactiveCommand<Unit, Unit>? BuildDeselectAll                 { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectThirdParties               { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectUsersSetup                 { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectStartUsuals                { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectBuildUsuals                { get; set; }
        public ReactiveCommand<Unit, Task>? ResetOcelotConfig                { get; set; }
        public ReactiveCommand<Unit, Task>? RefreshDockerContainerPanels     { get; set; }
        public ReactiveCommand<Unit, Unit>? VaultNotMockedEnv                { get; set; }
        public ReactiveCommand<Unit, Unit>? ProductionEnv                    { get; set; }
        public ReactiveCommand<Unit, Unit>? TraceLogEnv                      { get; set; }
        public ReactiveCommand<Unit, Unit>? AdjustLocalConfigs               { get; set; }
        public ReactiveCommand<Unit, Unit>? RevertLocalConfigs               { get; set; }
        public ReactiveCommand<Unit, Task>? DockerComposeBuildTraefik        { get; set; }
        public ReactiveCommand<Unit, Task>? ResetTraefikConfig               { get; set; }
        public ReactiveCommand<Unit, Task>? TraefikServicesFileSelection     { get; set; }

        public async Task OnContainerTabTappedAsync() => await UpdateDockerContainerPanelList();

        public void ClearOutput() => ConsoleOutputItems.Clear();

        public void SetOutput(string text)
        {
            ClearOutput();
            AddOutput(text);
        }

        public void AddOutput(string text) => ConsoleOutputItems.Add(text);

        public void SwitchBusy()
        {
            _window.PanelBusyBeacon?.SwitchBusy();
            _window.BuildBusyBeacon?.SwitchBusy();
            _window.OcelotBusyBeacon?.SwitchBusy();
            _window.TraefikBusyBeacon?.SwitchBusy();
        }

        public void SwitchIdle()
        {
            _window.PanelBusyBeacon?.SwitchIdle();
            _window.BuildBusyBeacon?.SwitchIdle();
            _window.OcelotBusyBeacon?.SwitchIdle();
            _window.TraefikBusyBeacon?.SwitchIdle();
        }

        private void InitializeCommands()
        {
            OpenBackendFolderSelection = ReactiveCommand.Create(
                async () =>
                {
                    var path = await Helper.ShowFolderSelection(_window, "Select backend repo folder");
                    if (!string.IsNullOrEmpty(path))
                    {
                        BackendRepoPath = path;
                    }
                }
            );

            OpenFrontendFolderSelection = ReactiveCommand.Create(
                async () =>
                {
                    var path = await Helper.ShowFolderSelection(_window, "Select frontend repo folder");
                    if (!string.IsNullOrEmpty(path))
                    {
                        FrontendRepoPath = path;
                    }
                }
            );

            OcelotConfigurationFileSelection = ReactiveCommand.Create(
                async () =>
                {
                    var files = await Helper.ShowJsonFileSelection(_window, "Select ocelotConfiguration.json");
                    if (files.Length > 0)
                    {
                        OcelotConfigurationPath = files[0];
                    }

                    if (!string.IsNullOrWhiteSpace(OcelotConfigurationPath))
                    {
                        Helper.UpdateOcelotItemList(_window);
                    }
                }
            );

            TraefikServicesFileSelection = ReactiveCommand.Create(
                async () =>
                {
                    var files = await Helper.ShowYamlFileSelection(_window, "Select traefik services-yaml");
                    if (files.Length > 0)
                    {
                        TraefikServicesPath = files[0];
                    }

                    if (!string.IsNullOrWhiteSpace(TraefikServicesPath))
                    {
                        Helper.UpdateTraefikItemList(_window);
                    }
                }
            );

            SaveConfiguration = ReactiveCommand.Create(
                () =>
                {
                    WriteConfig();
                    if (!string.IsNullOrWhiteSpace(BackendDockerComposePath) && !string.IsNullOrWhiteSpace(BackendDockerComposeOverridePath))
                    {
                        Helper.UpdateServiceNameLists(_window);
                    }
                }
            );

            DockerComposeUpDelayed = ReactiveCommand.Create(
                async () =>
                {
                    if (!SelectedServiceNames.Any()) return;

                    var basicCommand = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(BackendDockerComposePath, BackendDockerComposeOverridePath),
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
                        await Helper.ExecuteCliCommandAsync(firstBatchCommand, _window);

                        if (secondBatch.Any() || rest.Any())
                        {
                            await Task.Delay(TimeSpan.FromSeconds(_firstBatchWait));
                        }
                    }

                    if (secondBatch.Any())
                    {
                        var secondBatchCommand = Helper.ConcatCommand(basicCommand, secondBatch);
                        await Helper.ExecuteCliCommandAsync(secondBatchCommand, _window);

                        if (rest.Any())
                        {
                            await Task.Delay(TimeSpan.FromSeconds(_secondBatchWait));
                        }
                    }

                    if (!rest.Any()) return;

                    var command = Helper.ConcatCommand(basicCommand, rest);
                    await Helper.ExecuteCliCommandAsync(command, _window);
                }
            );

            DockerComposeUp = ReactiveCommand.Create(async () => await DockerComposeUpCommandExecution.Instance.ExecuteAsync(_window));

            DockerComposeStop = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(BackendDockerComposePath, BackendDockerComposeOverridePath),
                        "stop"
                    );

                    await Helper.ExecuteCliCommandAsync(command, _window);
                }
            );

            DockerComposeStart = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(BackendDockerComposePath, BackendDockerComposeOverridePath),
                        "start"
                    );

                    await Helper.ExecuteCliCommandAsync(Helper.ConcatCommand(command), _window);
                }
            );

            DockerComposeDown = ReactiveCommand.Create(async () => await DockerComposeDownCommandExecution.Instance.ExecuteAsync(_window));
            DockerComposeBuildOcelot = ReactiveCommand.Create(async () => await BuildOcelotCommandExecution.Instance.ExecuteAsync(_window));
            ResetOcelotConfig = ReactiveCommand.Create(async () => await ResetOcelotConfigCommandExecution.Instance.ExecuteAsync(_window));
            DockerPs = ReactiveCommand.Create(async () => await Helper.ExecuteCliCommandAsync(Helper.ConcatCommand("docker", "ps"), _window, false));
            DockerPsExited = ReactiveCommand.Create(
                async () => await Helper.ExecuteCliCommandAsync(
                                Helper.ConcatCommand("docker", "ps", "-a"),
                                _window,
                                false,
                                s => s.Contains("CONTAINER") || s.Contains("Exited")
                            )
            );

            DockerDbResetPrune = ReactiveCommand.Create(async () => await DockerDbResetPruneCommandExecution.Instance.ExecuteAsync(_window));

            DockerBuildAllConfirmation = ReactiveCommand.Create(
                async () =>
                {
                    var result = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(
                                                     new MessageBoxStandardParams
                                                     {
                                                         ContentTitle          = "Build",
                                                         ContentMessage        = "Sure you want to build all?",
                                                         ButtonDefinitions     = ButtonEnum.YesNo,
                                                         WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                                         Icon                  = Icon.Warning
                                                     }
                                                 )
                                                 .Show();

                    if (result == ButtonResult.Yes)
                    {
                        await DockerBuildAllCommandExecution.Instance.ExecuteAsync(_window);
                    }
                }
            );

            BackendSelectedBuild = ReactiveCommand.Create(async () => await BackendSelectedBuildCommandExecution.Instance.ExecuteAsync(_window));

            FrontendBuild = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(FrontendDockerComposePath, FrontendDockerComposeOverridePath),
                        "build"
                    );

                    await Helper.ExecuteCliCommandAsync(command, _window);
                }
            );

            AdjustLocalConfigs               = ReactiveCommand.Create(AdjustFrontendBackendLocalConfigs);
            RevertLocalConfigs               = ReactiveCommand.Create(RevertFrontendBackendLocalConfigs);
            FrontendAdjustDevConfigLocalhost = ReactiveCommand.Create(AdjustDevConfigLocalhost);
            FrontendAdjustDevConfigDevServer = ReactiveCommand.Create(AdjustDevConfigDevServer);
            FrontendRemoveModules            = ReactiveCommand.Create(RemoveFrontendModulePageFolders);

            FrontendDockerComposeUp = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(FrontendDockerComposePath, FrontendDockerComposeOverridePath),
                        "up",
                        "-d"
                    );

                    await Helper.ExecuteCliCommandAsync(command, _window);
                }
            );

            FrontendDockerComposeDown = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(FrontendDockerComposePath, FrontendDockerComposeOverridePath),
                        "down"
                    );

                    await Helper.ExecuteCliCommandAsync(command, _window);
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

            BuildDeselectAll = ReactiveCommand.Create(
                () =>
                {
                    foreach (var button in _window.BuildSelectionToggleButtons)
                    {
                        button.UnCheck();
                    }
                }
            );

            BuildSelectAll = ReactiveCommand.Create(
                () =>
                {
                    foreach (var button in _window.BuildSelectionToggleButtons)
                    {
                        button.Check();
                    }
                }
            );

            SelectThirdParties = ReactiveCommand.Create(
                () => Helper.SelectMatchingContents(_window.ServiceSelectionCheckBoxes, Helper.SplitCommaSeparated(ThirdParties))
            );

            SelectUsersSetup = ReactiveCommand.Create(
                () => Helper.SelectMatchingContents(_window.ServiceSelectionCheckBoxes, Helper.SplitCommaSeparated(UsersSetup))
            );

            SelectStartUsuals = ReactiveCommand.Create(
                () => Helper.SelectMatchingContents(_window.ServiceSelectionCheckBoxes, Helper.SplitCommaSeparated(StartUsuals))
            );

            SelectBuildUsuals = ReactiveCommand.Create(
                () => Helper.SelectMatchingContents(_window.BuildSelectionToggleButtons, Helper.SplitCommaSeparated(BuildUsuals))
            );

            RefreshDockerContainerPanels = ReactiveCommand.Create(async () => await UpdateDockerContainerPanelList());

            VaultNotMockedEnv = ReactiveCommand.Create(SetVaultNotMockedBackendEnvVariables);
            ProductionEnv     = ReactiveCommand.Create(SetProductionBackendEnvVariables);
            TraceLogEnv       = ReactiveCommand.Create(SetTraceLogEnvVariables);

            DockerComposeBuildTraefik = ReactiveCommand.Create(async () => await BuildTraefikCommandExecution.Instance.ExecuteAsync(_window));
            ResetTraefikConfig        = ReactiveCommand.Create(async () => await ResetTraefikConfigCommandExecution.Instance.ExecuteAsync(_window));
        }

        private void WriteConfig()
        {
            var appConfig = new AppConfig();
            appConfig.MapFrom(this);

            File.WriteAllText(App.ConfigFileName, JsonConvert.SerializeObject(appConfig, Formatting.Indented));
        }

        private void AdjustFrontendBackendLocalConfigs()
        {
            AdjustFrontendWebDevConfig("http://localhost");
            SetSslInactiveBackendEnvVariables();

            _window.ViewModel.SetOutput($"Config files adjusted");
        }

        private void RevertFrontendBackendLocalConfigs()
        {
            AdjustFrontendWebDevConfig("http://10.10.0.51");
            ReplaceInBackendDotEnv(
                ("SSL_ACTIVE=\"false\"", "SSL_ACTIVE=\"true\""),
                ("VAULT_IS_MOCKED=\"false\"", "VAULT_IS_MOCKED=\"true\""),
                ("VAULT_IS_VAULTCONFIGOVERRIDE=\"true\"", "VAULT_IS_VAULTCONFIGOVERRIDE=\"false\""),
                ("ASPNETCORE_ENVIRONMENT=\"Production\"", "ASPNETCORE_ENVIRONMENT=\"Development\""),
                ("ENVIRONMENT_TRACE_LOG=\"true\"", "ENVIRONMENT_TRACE_LOG=\"false\"")
            );
        }

        private void AdjustDevConfigLocalhost() => AdjustDevConfigs("https://localhost");

        private void AdjustDevConfigDevServer() => AdjustDevConfigs($"https://{DevServerIp}");

        private void AdjustDevConfigs(string url)
        {
            AdjustFrontendWebDevConfig(url);
            // AdjustClientInstituteConfig(host);

            _window.ViewModel.SetOutput($"Config files adjusted to url {url}");
        }

        private void AdjustFrontendWebDevConfig(string url)
        {
            var path = Path.Join(FrontendRepoPath, "src", "assets", "config", "config.dev.json");
            var text = File.ReadAllText(path, _encoding);
            text = Regex.Replace(text, "\"baseUrl\": \"http(s)*:\\/\\/(.*)\",", $"\"baseUrl\": \"{url}\",");
            File.WriteAllText(path, text, _encoding);
        }

        private void AdjustClientInstituteConfig(string host)
        {
            var path = Path.Join(FrontendRepoPath, "electron", "config", "system", "config", "institute_config.json");
            var text = File.ReadAllText(path, _encoding);
            text = Regex.Replace(text, "\"baseUrl\": \"http:\\/\\/(.*)\",", $"\"baseUrl\": \"http://{host}\",");
            File.WriteAllText(path, text, _encoding);
        }

        private void RemoveFrontendModulePageFolders()
        {
            RemoveModulePageFolders();
            AdjustRoutingFile();

            _window.ViewModel.SetOutput("Module page folders removed");
        }

        private void RemoveModulePageFolders()
        {
            var noDelete          = new[] { "kneeMRT", "spineMRT", "shared" };
            var moduleFolderPaths = Directory.EnumerateDirectories(Path.Join(FrontendRepoPath, "src", "app", "pages")).Where(p => !noDelete.Any(p.Contains));
            foreach (var folderPath in moduleFolderPaths)
            {
                var di = new DirectoryInfo(folderPath);
                if (di.Exists)
                {
                    di.Delete(true);
                }
            }
        }

        private void AdjustRoutingFile()
        {
            var routingFilePath = Path.Join(FrontendRepoPath, "src", "app", "shared", "authorized", "authorized-routing.module.ts");
            var lines           = File.ReadAllLines(routingFilePath);
            if (lines.Length < 183) return;

            var newLines = new List<string>();
            newLines.AddRange(lines.Take(25));
            newLines.AddRange(lines.Take(41).Skip(33));
            newLines.AddRange(lines.Skip(117));
            File.WriteAllLines(routingFilePath, newLines);
        }

        private void SetSslInactiveBackendEnvVariables() => ReplaceInBackendDotEnv(("SSL_ACTIVE=\"true\"", "SSL_ACTIVE=\"false\""));

        private void SetVaultNotMockedBackendEnvVariables() => ReplaceInBackendDotEnv(
            ("VAULT_IS_MOCKED=\"true\"", "VAULT_IS_MOCKED=\"false\""),
            ("VAULT_IS_VAULTCONFIGOVERRIDE=\"false\"", "VAULT_IS_VAULTCONFIGOVERRIDE=\"true\"")
        );

        private void SetProductionBackendEnvVariables() =>
            ReplaceInBackendDotEnv(("ASPNETCORE_ENVIRONMENT=\"Development\"", "ASPNETCORE_ENVIRONMENT=\"Production\""));

        private void SetTraceLogEnvVariables() => ReplaceInBackendDotEnv(("ENVIRONMENT_TRACE_LOG=\"false\"", "ENVIRONMENT_TRACE_LOG=\"true\""));

        private void ReplaceInBackendDotEnv(params (string orig, string replacement)[] replacements)
        {
            var path = Path.Join(BackendRepoPath, ".env");
            var text = File.ReadAllText(path, _encoding);
            foreach (var (orig, replacement) in replacements)
            {
                text = text.Replace(orig, replacement);
            }

            File.WriteAllText(path, text, _encoding);
            _window.ViewModel.SetOutput($"Env file adjusted");
        }

        private async Task UpdateDockerContainerPanelList()
        {
            var containerPanelContainer = _window.DockerContainerPanelContainer ?? throw new InvalidOperationException();
            containerPanelContainer.Children.Clear();
            if (_window.DockerApiStatus != null) _window.DockerApiStatus.Text = string.Empty;

            var containersApi = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
            var containers = containersApi.Select(
                                              c => new DockerContainer
                                              {
                                                  Id    = c.ID,
                                                  Name  = c.Names.First().TrimStart('/'),
                                                  State = c.State
                                              }
                                          )
                                          .OrderBy(c => c.Name)
                                          .ToList();

            const double nameWidth    = 400.0;
            const double stateWidth   = 70.0;
            const double buttonWidth  = 70.0;
            var          buttonMargin = new Thickness(4, 0, 4, 0);
            foreach (var container in containers)
            {
                var stack = new StackPanel
                {
                    Orientation       = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin            = new Thickness(4, 8, 8, 4)
                };

                var nameBlock = new TextBlock
                {
                    Text              = container.Name,
                    Width             = nameWidth,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                var stateBlock = new TextBlock
                {
                    Text              = container.State,
                    Width             = stateWidth,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                var stopButton = new Button
                {
                    Content                    = "Stop",
                    Width                      = buttonWidth,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin                     = buttonMargin
                };

                stopButton.Click += async (_, _) => await ExecuteDockerCommandHandler(
                                                        () => ExecuteDockerApiCommand(
                                                            () => _dockerClient.Containers.StopContainerAsync(container.Id, new ContainerStopParameters())
                                                        ),
                                                        $"Container {container.Name} stopped"
                                                    );

                var startButton = new Button
                {
                    Content                    = "Start",
                    Width                      = buttonWidth,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin                     = buttonMargin
                };

                startButton.Click += async (_, _) => await ExecuteDockerCommandHandler(
                                                         () => ExecuteDockerApiCommand(
                                                             () => _dockerClient.Containers.StartContainerAsync(container.Id, new ContainerStartParameters())
                                                         ),
                                                         $"Container {container.Name} started"
                                                     );

                var restartButton = new Button
                {
                    Content                    = "Restart",
                    Width                      = buttonWidth,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin                     = buttonMargin
                };

                restartButton.Click += async (_, _) => await ExecuteDockerCommandHandler(
                                                           () => ExecuteDockerApiCommand(
                                                               () => _dockerClient.Containers.RestartContainerAsync(
                                                                   container.Id,
                                                                   new ContainerRestartParameters()
                                                               )
                                                           ),
                                                           $"Container {container.Name} restarted"
                                                       );

                var removeButton = new Button
                {
                    Content                    = "Remove",
                    Width                      = buttonWidth,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin                     = buttonMargin
                };

                removeButton.Click += async (_, _) => await ExecuteDockerCommandHandler(
                                                          () => ExecuteDockerApiCommand(
                                                              () => _dockerClient.Containers.RemoveContainerAsync(
                                                                  container.Id,
                                                                  new ContainerRemoveParameters { Force = true }
                                                              )
                                                          ),
                                                          $"Container {container.Name} removed"
                                                      );

                stack.Children.AddRange(new Control[] { nameBlock, stateBlock, stopButton, startButton, restartButton, removeButton });
                containerPanelContainer.Children.Add(stack);
            }
        }

        private async Task ExecuteDockerCommandHandler(Func<Task<bool>> action, string successMessage)
        {
            var success = await action();
            if (success)
            {
                if (_window.DockerApiStatus != null) _window.DockerApiStatus.Text = successMessage;
            }
        }

        private static async Task<bool> ExecuteDockerApiCommand(Func<Task> action)
        {
            try
            {
                await action();

                return true;
            }
            catch (DockerApiException exception)
            {
                var message = JObject.Parse(exception.ResponseBody)["message"]?.ToString() ?? exception.ResponseBody;
                MessageBoxHelper.ShowErrorMessage(message, "Invalid operation");

                return false;
            }
        }

        private static async Task<bool> ExecuteDockerApiCommand(Func<Task<bool>> action)
        {
            try
            {
                return await action();
            }
            catch (DockerApiException exception)
            {
                var message = JObject.Parse(exception.ResponseBody)["message"]?.ToString() ?? exception.ResponseBody;
                MessageBoxHelper.ShowErrorMessage(message, "Invalid operation");

                return false;
            }
        }
    }
}
