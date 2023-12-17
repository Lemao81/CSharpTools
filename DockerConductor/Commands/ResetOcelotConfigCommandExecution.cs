using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Extensions;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class ResetOcelotConfigCommandExecution : BuildCommandExecutionBase
    {
        public static ResetOcelotConfigCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            await ResetAndSaveOcelotAsync(window);
            window.OcelotRouteUis.ForEach(ui => ui.RadioButton80.Check());
            window.TabControl.SwitchToTab(TabItemIndex.Panel);
            await ExecuteBuildAsync(window, ServiceNames.OcelotApiGateway);
        }

        private static async Task ResetAndSaveOcelotAsync(MainWindow window)
        {
            var isChanged = false;
            foreach (var route in window.ViewModel.OcelotRoutes.Where(r => !string.IsNullOrEmpty(r.OrigHost)))
            {
                route.ReplaceHost(route.OrigHost!, window.ViewModel.OcelotConfigLines);
                route.ReplacePort(80, window.ViewModel.OcelotConfigLines);
                route.OrigHost = null;
                isChanged      = true;
            }

            if (isChanged)
            {
                await File.WriteAllLinesAsync(window.ViewModel.OcelotConfigurationPath, window.ViewModel.OcelotConfigLines);
            }
        }
    }
}
