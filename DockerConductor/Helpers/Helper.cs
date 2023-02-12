using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using DockerConductor.Models;
using DockerConductor.Services;
using DockerConductor.ViewModels;
using DockerConductor.Views;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DockerConductor.Helpers
{
    public static class Helper
    {
        private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder().IgnoreUnmatchedProperties()
                                                                                          .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                                                                          .Build();

        public static IEnumerable<string> ExcludeByCommaSeparated(IEnumerable<string> strings, string commaseparated)
        {
            var excludes = SplitCommaSeparated(commaseparated);

            return strings.Where(s => excludes.All(e => string.IsNullOrWhiteSpace(e) || !s.Contains(e, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static IEnumerable<string> FilterByCommaSeparated(IEnumerable<string> strings, string commaseparated)
        {
            var filters = SplitCommaSeparated(commaseparated);

            return strings.Where(s => filters.Any(f => f.Contains(s, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static async Task ExecuteCliCommand(string command, MainWindow window)
        {
            if (window.ConsoleOutput is null) return;

            window.ViewModel.ExecutedCommand = command;
            window.ConsoleOutput.Text        = string.Empty;

            var startInfo = new ProcessStartInfo("cmd.exe", $"/C {command}")
            {
                WindowStyle            = ProcessWindowStyle.Hidden,
                CreateNoWindow         = true,
                UseShellExecute        = false,
                WorkingDirectory       = @"C:\Windows\System32",
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true
            };

            var process = new Process();
            process.StartInfo          =  startInfo;
            process.OutputDataReceived += OnOutputReceived();
            process.ErrorDataReceived  += OnOutputReceived();
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            process.Close();

            DataReceivedEventHandler OnOutputReceived() => (_, args) =>
                                                           {
                                                               if (args.Data != null)
                                                               {
                                                                   Dispatcher.UIThread.InvokeAsync(
                                                                       () =>
                                                                       {
                                                                           window.ConsoleOutput.Text += args.Data + "\n";
                                                                           window.ConsoleScrollViewer?.ScrollToEnd();
                                                                       }
                                                                   );
                                                               }
                                                           };
        }

        public static void UpdateServiceCheckboxList(MainWindow window)
        {
            var dockerComposeText         = File.ReadAllText(window.ViewModel.BackendDockerComposePath) ?? throw new InvalidOperationException();
            var dockerComposeOverrideText = File.ReadAllText(window.ViewModel.BackendDockerComposeOverridePath) ?? throw new InvalidOperationException();
            var dockerCompose             = YamlDeserializer.Deserialize<DockerCompose>(dockerComposeText) ?? throw new InvalidOperationException();
            var dockerComposeOverride     = YamlDeserializer.Deserialize<DockerCompose>(dockerComposeOverrideText) ?? throw new InvalidOperationException();
            foreach (var key in dockerComposeOverride.Services.Keys.Where(k => !dockerCompose.Services.ContainsKey(k)))
            {
                dockerCompose.Services[key] = dockerComposeOverride.Services[key];
            }

            var serviceNames              = ExcludeByCommaSeparated(dockerCompose.Services.Keys, window.ViewModel.Excludes).OrderBy(SortServices);
            var serviceSelectionContainer = window.ServiceSelectionContainer ?? throw new InvalidOperationException();

            serviceSelectionContainer.Children.Clear();
            foreach (var serviceName in serviceNames)
            {
                var checkBox = new CheckBox { Content = serviceName };
                serviceSelectionContainer.Children.Add(checkBox);
            }

            int SortServices(string name)
            {
                if (!name.Contains("radio")) return 1;

                if (name.Contains("core")) return 2;

                if (name.Contains("knee")) return 3;

                if (name.Contains("shoulder")) return 4;

                if (name.Contains("thoraxct")) return 5;

                return 6;
            }
        }

        public static void UpdateOcelotItemList(MainWindow window)
        {
            var ocelotConfigString = File.ReadAllText(window.ViewModel.OcelotConfigurationPath) ?? throw new InvalidOperationException();
            window.ViewModel.OcelotConfigString = ocelotConfigString;
            var ocelotConfig = JObject.Parse(ocelotConfigString);
            window.ViewModel.OcelotConfig = ocelotConfig;
            var itemsToShow = ocelotConfig["Routes"]?.Where(IsRouteToConsider);

            if (itemsToShow is null) return;

            var ocelotItemsContainer = window.OcelotItemContainer ?? throw new InvalidOperationException();

            ocelotItemsContainer.Children.Clear();
            window.OcelotRouteUis.Clear();
            foreach (var item in itemsToShow.Reverse())
            {
                var itemContainer = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin      = new Thickness(0, 0, 0, 8)
                };

                var swaggerKey = item["SwaggerKey"]?.ToString();
                var origHost   = (item["DownstreamHostAndPorts"] as JArray)?.First?["Host"]?.ToString() ?? string.Empty;

                var uiModel = new OcelotRouteUi
                {
                    NameTextBlock = new TextBlock
                    {
                        Text              = swaggerKey,
                        Width             = 200,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    RadioButton80 = new RadioButton
                    {
                        Content   = "80",
                        Margin    = new Thickness(0, 0, 16, 0),
                        IsChecked = true
                    },
                    RadioButton5000 = new RadioButton
                    {
                        Content = "5000",
                        Margin  = new Thickness(0, 0, 16, 0)
                    },
                    RadioButton5001 = new RadioButton
                    {
                        Content = "5001",
                        Margin  = new Thickness(0, 0, 16, 0)
                    },
                    RadioButton5002 = new RadioButton { Content = "5002" },
                    OrigHost        = origHost
                };

                itemContainer.Children.Add(uiModel.NameTextBlock);
                itemContainer.Children.Add(uiModel.RadioButton80);
                itemContainer.Children.Add(uiModel.RadioButton5000);
                itemContainer.Children.Add(uiModel.RadioButton5001);
                itemContainer.Children.Add(uiModel.RadioButton5002);
                ocelotItemsContainer.Children.Add(itemContainer);
                window.OcelotRouteUis.Add(uiModel);
            }

            window.ViewModel.OcelotConfigLines = File.ReadAllLines(window.ViewModel.OcelotConfigurationPath);
            window.ViewModel.OcelotParseRoutes.Clear();
            window.ViewModel.OcelotParseRoutes.AddRange(new OcelotConfigurationParser().Parse(window.ViewModel.OcelotConfigLines));
        }

        public static string ConcatCommand(params string[] parts)
        {
            var nonEmpties = parts.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim());

            return string.Join(" ", nonEmpties);
        }

        public static string ConcatCommand(string start, IEnumerable<string> strings)
        {
            var nonEmpties = strings.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim());

            return start + " " + string.Join(" ", nonEmpties);
        }

        public static string ConcatFilePathArguments(params string[] paths) => string.Join(
            " ",
            paths.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => $"-f \"{p}\"")
        );

        public static IEnumerable<string> SplitCommaSeparated(string str) => str.Split(",").Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));

        public static void SelectMatchingContents(IEnumerable<CheckBox> checkBoxes, IEnumerable<string> strings)
        {
            foreach (var checkBox in checkBoxes.Where(
                         c => strings.Any(t => c.Content != null && c.Content.ToString()!.Contains(t, StringComparison.InvariantCultureIgnoreCase))
                     ))
            {
                checkBox.IsChecked = true;
            }
        }

        public static async Task<string[]> ShowYamlFileSelection(Window window, string title) => await ShowFileSelection(window, title, "yaml", "yml", "yaml");

        public static async Task<string[]> ShowJsonFileSelection(Window window, string title) => await ShowFileSelection(window, title, "json", "json");

        public static async Task<string> ShowFolderSelection(Window window, string title)
        {
            var folderDialog = new OpenFolderDialog { Title = title };

            return await folderDialog.ShowAsync(window);
        }

        private static async Task<string[]> ShowFileSelection(Window window, string title, string filterName, params string[] filterExtensions)
        {
            var fileDialog = new OpenFileDialog { AllowMultiple = false };
            fileDialog.Filters.Add(
                new FileDialogFilter
                {
                    Name       = filterName,
                    Extensions = new List<string>(filterExtensions)
                }
            );

            fileDialog.Title = title;
            var files = await fileDialog.ShowAsync(window);

            return files;
        }

        private static bool IsRouteToConsider(JToken r) => !string.IsNullOrWhiteSpace(r["SwaggerKey"]?.ToString())
                                                           && r["DownstreamHostAndPorts"] is JArray downStreams
                                                           && downStreams.Any()
                                                           && !string.IsNullOrWhiteSpace(downStreams.First!["Host"]?.ToString());
    }
}
