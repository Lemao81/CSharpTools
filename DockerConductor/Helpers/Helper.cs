using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using DockerConductor.ViewModels;
using DockerConductor.Views;
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
            var excludes = commaseparated.Split(",").Select(s => s.Trim());

            return strings.Where(s => excludes.All(e => string.IsNullOrWhiteSpace(e) || !s.Contains(e, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static IEnumerable<string> FilterByCommaSeparated(IEnumerable<string> strings, string commaseparated)
        {
            var filters = commaseparated.Split(",").Select(s => s.Trim());

            return strings.Where(s => filters.Any(f => f.Contains(s, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static async Task ExecuteCliCommand(string command)
        {
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
            process.StartInfo = startInfo;
            Console.WriteLine("Executing: " + command);
            process.Start();
            await process.WaitForExitAsync();
            Console.WriteLine(await process.StandardOutput.ReadToEndAsync());
            Console.WriteLine(await process.StandardError.ReadToEndAsync());
            Console.WriteLine("Command executed");
        }

        public static void UpdateServiceCheckboxList(MainWindow window)
        {
            var dockerComposeText         = File.ReadAllText(window.ViewModel.DockerComposePath) ?? throw new InvalidOperationException();
            var dockerCompose             = YamlDeserializer.Deserialize<DockerCompose>(dockerComposeText) ?? throw new InvalidOperationException();
            var serviceNames              = ExcludeByCommaSeparated(dockerCompose.Services.Keys, window.ViewModel.Excludes);
            var serviceSelectionContainer = window.ServiceSelectionContainer ?? throw new InvalidOperationException();

            serviceSelectionContainer.Children.Clear();
            foreach (var serviceName in serviceNames)
            {
                var checkBox = new CheckBox { Content = serviceName };
                serviceSelectionContainer.Children.Add(checkBox);
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
    }
}
