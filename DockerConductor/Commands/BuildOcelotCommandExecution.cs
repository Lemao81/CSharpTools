using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Extensions;
using DockerConductor.Models;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class BuildOcelotCommandExecution : BuildCommandExecutionBase
    {
        public static BuildOcelotCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            await ReplaceAndSaveOcelotAsync(window);
            window.TabControl.SwitchToTab(TabItemIndex.Panel);
            await ExecuteBuildAsync(window, ServiceNames.OcelotApiGateway);
        }

        private static async Task ReplaceAndSaveOcelotAsync(MainWindow window)
        {
            foreach (var route in window.ViewModel.OcelotRoutes)
            {
                var uiModel = window.OcelotRouteUis.SingleOrDefault(u => u.Name == route.Name);
                if (uiModel is null) continue;

                if (uiModel.IsInternalHost)
                {
                    if (string.IsNullOrEmpty(route.OrigHost))
                    {
                        route.OrigHost = uiModel.OrigHost;
                    }

                    route.ReplaceHost(Consts.InternalHost, window.ViewModel.OcelotConfigLines);
                    route.ReplacePort(uiModel.Port, window.ViewModel.OcelotConfigLines);
                    AlignWebSocketEntryIfExist(uiModel, Consts.InternalHost, true);
                }
                else if (!string.IsNullOrEmpty(route.OrigHost))
                {
                    route.ReplaceHost(route.OrigHost, window.ViewModel.OcelotConfigLines);
                    route.ReplacePort(80, window.ViewModel.OcelotConfigLines);
                    AlignWebSocketEntryIfExist(uiModel, route.OrigHost, false);
                }
            }

            await File.WriteAllLinesAsync(window.ViewModel.OcelotConfigurationPath, window.ViewModel.OcelotConfigLines);

            void AlignWebSocketEntryIfExist(RouteUi uiModel, string host, bool isInternalHost)
            {
                var webSocketRoute = window.ViewModel.OcelotRoutes.FirstOrDefault(
                    r => r.Path.Contains(uiModel.Name, StringComparison.InvariantCultureIgnoreCase) && r.IsWebSocket
                );

                if (webSocketRoute == null) return;

                webSocketRoute.OrigHost = isInternalHost ? webSocketRoute.Host : null;
                webSocketRoute.ReplaceHost(host, window.ViewModel.OcelotConfigLines);
                webSocketRoute.ReplacePort(uiModel.Port, window.ViewModel.OcelotConfigLines);
            }
        }
    }
}
