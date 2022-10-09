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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Layout;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerConductor.Constants;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json.Linq;

namespace DockerConductor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow                 _window;
        private          string                     _excludes     = string.Empty;
        private          string                     _thirdParties = string.Empty;
        private          string                     _usuals       = string.Empty;
        private          string                     _firstBatch   = string.Empty;
        private          int                        _firstBatchWait;
        private          string                     _secondBatch = string.Empty;
        private          int                        _secondBatchWait;
        private          string                     _excludesStop              = string.Empty;
        private          string                     _dbVolume                  = string.Empty;
        public           string                     _devServerIp               = string.Empty;
        private          string                     _ocelotConfigurationPath   = string.Empty;
        private readonly Dictionary<string, string> _ocelotConfigOrigHostCache = new();
        private          string                     _executedCommand           = string.Empty;
        private          string                     _frontendRepoPath          = string.Empty;
        private          string                     _backendRepoPath           = string.Empty;
        private          DockerClient               _dockerClient;

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

        public IEnumerable<string> LastSelected       { get; set; } = Enumerable.Empty<string>();
        public JObject?            OcelotConfig       { get; set; }
        public string              OcelotConfigString { get; set; }

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
        public ReactiveCommand<Unit, Task>? DockerDbResetPrune               { get; set; }
        public ReactiveCommand<Unit, Task>? DockerBuildConfirmation          { get; set; }
        public ReactiveCommand<Unit, Task>? FrontendDockerComposeUp          { get; set; }
        public ReactiveCommand<Unit, Task>? FrontendDockerComposeDown        { get; set; }
        public ReactiveCommand<Unit, Task>? FrontendBuild                    { get; set; }
        public ReactiveCommand<Unit, Unit>? FrontendAdjustDevConfigLocalhost { get; set; }
        public ReactiveCommand<Unit, Unit>? FrontendAdjustDevConfigDevServer { get; set; }
        public ReactiveCommand<Unit, Unit>? FrontendRemoveModules            { get; set; }
        public ReactiveCommand<Unit, Unit>? DeselectAll                      { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectThirdParties               { get; set; }
        public ReactiveCommand<Unit, Unit>? SelectUsuals                     { get; set; }
        public ReactiveCommand<Unit, Unit>? ResetOcelotConfig                { get; set; }
        public ReactiveCommand<Unit, Task>? RefreshDockerContainerPanels     { get; set; }

        public async Task OnContainerTabTappedAsync()
        {
            await UpdateDockerContainerPanelList();
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

            SaveConfiguration = ReactiveCommand.Create(
                () =>
                {
                    WriteConfig();
                    if (!string.IsNullOrWhiteSpace(BackendDockerComposePath) && !string.IsNullOrWhiteSpace(BackendDockerComposeOverridePath))
                    {
                        Helper.UpdateServiceCheckboxList(_window);
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

            DockerComposeUp = ReactiveCommand.Create(
                async () =>
                {
                    var basicCommand = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(BackendDockerComposePath, BackendDockerComposeOverridePath),
                        "up",
                        "-d",
                        "--no-deps"
                    );

                    await Helper.ExecuteCliCommand(Helper.ConcatCommand(basicCommand, SelectedServiceNames), _window);
                }
            );

            DockerComposeStop = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(BackendDockerComposePath, BackendDockerComposeOverridePath),
                        "stop"
                    );

                    await Helper.ExecuteCliCommand(command, _window);
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

                    await Helper.ExecuteCliCommand(Helper.ConcatCommand(command), _window);
                }
            );

            DockerComposeDown = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(BackendDockerComposePath, BackendDockerComposeOverridePath),
                        "down"
                    );

                    await Helper.ExecuteCliCommand(command, _window);
                }
            );

            DockerComposeBuildOcelot = ReactiveCommand.Create(
                async () =>
                {
                    SaveOcelot();
                    var basicCommand = GetBasicBuildCommand();
                    var command      = Helper.ConcatCommand(basicCommand, "ocelotapigateway");
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
                        var basicCommand = GetBasicBuildCommand();
                        var command      = Helper.ConcatCommand(basicCommand, SelectedServiceNames);
                        await Helper.ExecuteCliCommand(command, _window);
                    }
                }
            );

            FrontendBuild = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(FrontendDockerComposePath, FrontendDockerComposeOverridePath),
                        "build"
                    );

                    await Helper.ExecuteCliCommand(command, _window);
                }
            );

            FrontendAdjustDevConfigLocalhost = ReactiveCommand.Create(AdjustDevConfigLocalhost);

            FrontendAdjustDevConfigDevServer = ReactiveCommand.Create(AdjustDevConfigDevServer);

            FrontendRemoveModules = ReactiveCommand.Create(RemoveFrontendModulePageFolders);

            FrontendDockerComposeUp = ReactiveCommand.Create(
                async () =>
                {
                    var command = Helper.ConcatCommand(
                        Consts.DockerCompose,
                        Helper.ConcatFilePathArguments(FrontendDockerComposePath, FrontendDockerComposeOverridePath),
                        "up",
                        "-d"
                    );

                    await Helper.ExecuteCliCommand(command, _window);
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

                    await Helper.ExecuteCliCommand(command, _window);
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

            ResetOcelotConfig = ReactiveCommand.Create(
                () =>
                {
                }
            );

            RefreshDockerContainerPanels = ReactiveCommand.Create(async () => await UpdateDockerContainerPanelList());
        }

        private string GetBasicBuildCommand()
        {
            var basicCommand = Helper.ConcatCommand(
                Consts.DockerCompose,
                Helper.ConcatFilePathArguments(BackendDockerComposePath, BackendDockerComposeOverridePath),
                "build"
            );

            return basicCommand;
        }

        private void WriteConfig()
        {
            var appConfig = new AppConfig();
            appConfig.MapFrom(this);

            File.WriteAllText(App.ConfigFileName, JsonConvert.SerializeObject(appConfig, Formatting.Indented));
        }

        private void SaveOcelot()
        {
            if (OcelotConfig?["Routes"] is not JArray routes) return;

            foreach (var routeToken in routes)
            {
                if (routeToken is not JObject route) return;

                var uiModel = _window.OcelotRouteUis.SingleOrDefault(u => u.Name == route["SwaggerKey"]?.ToString());
                if (uiModel is null) continue;

                if (uiModel.IsHost)
                {
                    var hostAndPort = (route["DownstreamHostAndPorts"] as JArray)?.FirstOrDefault();
                    if (hostAndPort is null) continue;

                    if (!_ocelotConfigOrigHostCache.ContainsKey(uiModel.Name))
                    {
                        _ocelotConfigOrigHostCache[uiModel.Name] = uiModel.OrigHost;
                    }

                    // radioreport-angiographymrt-api",\s+"Port": (\d+)
                    // var hostMatches = new Regex(uiModel.OrigHost).Match(OcelotConfigString);
                    // var portMatches = new Regex($"{uiModel.OrigHost}\",\\s+\"Port\": (\\d+)").Match(OcelotConfigString);
                    hostAndPort["Host"] = "host.docker.internal";
                    hostAndPort["Port"] = uiModel.Port;
                }
                else if (_ocelotConfigOrigHostCache.ContainsKey(uiModel.Name))
                {
                    var hostAndPort = (route["DownstreamHostAndPorts"] as JArray)?.FirstOrDefault();
                    if (hostAndPort is null) continue;

                    hostAndPort["Host"] = _ocelotConfigOrigHostCache[uiModel.Name];
                    hostAndPort["Port"] = 80;
                    _ocelotConfigOrigHostCache.Remove(uiModel.Name);
                }
            }

            File.WriteAllText(OcelotConfigurationPath, OcelotConfig.ToString(Formatting.Indented));
        }

        private void AdjustDevConfigLocalhost() => AdjustDevConfigs("localhost");

        private void AdjustDevConfigDevServer() => AdjustDevConfigs(DevServerIp);

        private void AdjustDevConfigs(string host)
        {
            AdjustWebDevConfig(host);
            AdjustClientInstituteConfig(host);

            if (_window.ConsoleOutput != null) _window.ConsoleOutput.Text = $"Config files adjusted to host {host}";
        }

        private void AdjustWebDevConfig(string host)
        {
            var path = Path.Join(FrontendRepoPath, "src", "assets", "config", "config.dev.json");
            var text = File.ReadAllText(path, Encoding.UTF8);
            text = Regex.Replace(text, "http:\\/\\/(.*)\",", $"http://{host}\",");
            File.WriteAllText(path, text, Encoding.UTF8);
        }

        private void AdjustClientInstituteConfig(string host)
        {
            var path = Path.Join(FrontendRepoPath, "electron", "config", "system", "config", "institute_config.json");
            var text = File.ReadAllText(path, Encoding.UTF8);
            text = Regex.Replace(text, "http:\\/\\/(.*)\",", $"http://{host}\",");
            File.WriteAllText(path, text, Encoding.UTF8);
        }

        private void RemoveFrontendModulePageFolders()
        {
            RemoveModulePageFolders();
            AdjustRoutingFile();

            if (_window.ConsoleOutput != null) _window.ConsoleOutput.Text = "Module page folders removed";
        }

        private void RemoveModulePageFolders()
        {
            var noDelete          = new[] { "kneeMRT", "shared" };
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
            newLines.AddRange(lines.Take(37).Skip(33));
            newLines.AddRange(lines.Skip(117));
            File.WriteAllLines(routingFilePath, newLines);
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
