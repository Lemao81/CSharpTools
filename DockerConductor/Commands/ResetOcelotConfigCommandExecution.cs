﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DockerConductor.Helpers;
using DockerConductor.Views;
using static DockerConductor.Helpers.DockerComposeCommandHelper;

namespace DockerConductor.Commands
{
    public static class ResetOcelotConfigCommandExecution
    {
        public static async Task ExecuteAsync(MainWindow window)
        {
            await ResetAndSaveOcelotAsync(window);
            window.OcelotRouteUis.ForEach(ui => ui.RadioButton80.IsChecked = true);
            await ExecuteBuildAsync(window);
        }

        private static async Task ResetAndSaveOcelotAsync(MainWindow window)
        {
            var isChanged = false;
            foreach (var route in window.ViewModel.OcelotRoutes.Where(r => !string.IsNullOrEmpty(r.OrigHost)))
            {
                route.ReplaceHost(route.OrigHost, window.ViewModel.OcelotConfigLines);
                route.ReplacePort(80, window.ViewModel.OcelotConfigLines);
                route.OrigHost = null;
                isChanged      = true;
            }

            if (isChanged)
            {
                await File.WriteAllLinesAsync(window.ViewModel.OcelotConfigurationPath, window.ViewModel.OcelotConfigLines);
            }
        }

        private static async Task ExecuteBuildAsync(MainWindow window)
        {
            var basicCommand = GetBasicBuildCommand(window.ViewModel.BackendDockerComposePath, window.ViewModel.BackendDockerComposeOverridePath);
            var command      = Helper.ConcatCommand(basicCommand, "ocelotapigateway");
            await Helper.ExecuteCliCommand(command, window);
        }
    }
}