using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Extensions;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class ResetTraefikConfigCommandExecution : BuildCommandExecutionBase
    {
        public static ResetTraefikConfigCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            await ResetAndSaveTraefikAsync(window);
            window.TabControl.SwitchToTab(TabItemIndex.Panel);
            await ExecuteBuildAsync(window, ServiceNames.Traefik);
        }

        private static async Task ResetAndSaveTraefikAsync(MainWindow window)
        {
            var isChanged = false;
            if (window.ViewModel.TraefikServicesHttp?.Http?.Services is not null)
            {
                foreach (var routeUi in window.TraefikRouteUis.Where(u => u.IsInternalHost))
                {
                    window.ViewModel.TraefikServicesHttp.Http.Services[routeUi.Name].LoadBalancer.Servers.First().Url = routeUi.OrigHost;
                    routeUi.RadioButton80.Check();
                    isChanged = true;
                }
            }

            if (isChanged && !string.IsNullOrWhiteSpace(window.ViewModel.TraefikServicesOrigText))
            {
                await File.WriteAllTextAsync(window.ViewModel.TraefikServicesPath, window.ViewModel.TraefikServicesOrigText);
            }
        }
    }
}
