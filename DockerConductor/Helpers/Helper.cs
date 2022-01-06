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

            window.ConsoleOutput.Text = $"Executing: '{command}'\n\n";

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
            var dockerComposeText = File.ReadAllText(window.ViewModel.DockerComposePath) ?? throw new InvalidOperationException();
            var dockerCompose     = YamlDeserializer.Deserialize<DockerCompose>(dockerComposeText) ?? throw new InvalidOperationException();
            var serviceNames = ExcludeByCommaSeparated(dockerCompose.Services.Keys, window.ViewModel.Excludes)
                .OrderBy(s => s.Contains("radio") ? s.Contains("core") ? 1 : 2 : 0);

            var serviceSelectionContainer = window.ServiceSelectionContainer ?? throw new InvalidOperationException();

            serviceSelectionContainer.Children.Clear();
            foreach (var serviceName in serviceNames)
            {
                var checkBox = new CheckBox { Content = serviceName };
                serviceSelectionContainer.Children.Add(checkBox);
            }
        }

        public static void UpdateOcelotItemList(MainWindow window)
        {
            var ocelotConfigText = File.ReadAllText(window.ViewModel.OcelotConfigurationPath) ?? throw new InvalidOperationException();
            var ocelotConfig     = JObject.Parse(ocelotConfigText);
            window.ViewModel.OcelotConfig = ocelotConfig;
            var itemsToShow = ocelotConfig["Routes"]?.Where(r => !string.IsNullOrEmpty(r["SwaggerKey"]?.ToString()));
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

                var uiModel = new OcelotRouteUi
                {
                    NameTextBlock = new TextBlock
                    {
                        Text              = item["SwaggerKey"]?.ToString(),
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
                    RadioButton5002 = new RadioButton { Content = "5002" }
                };

                itemContainer.Children.Add(uiModel.NameTextBlock);
                itemContainer.Children.Add(uiModel.RadioButton80);
                itemContainer.Children.Add(uiModel.RadioButton5000);
                itemContainer.Children.Add(uiModel.RadioButton5001);
                itemContainer.Children.Add(uiModel.RadioButton5002);
                ocelotItemsContainer.Children.Add(itemContainer);
                window.OcelotRouteUis.Add(uiModel);
            }
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

        public static async Task<string[]> ShowFileSelection(Window window, string title, string filterName, params string[] filterExtensions)
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

        public static async Task<string[]> ShowYamlFileSelection(Window window, string title) => await ShowFileSelection(window, title, "yaml", "yml", "yaml");

        public static async Task<string[]> ShowJsonFileSelection(Window window, string title) => await ShowFileSelection(window, title, "json", "json");
    }
}
