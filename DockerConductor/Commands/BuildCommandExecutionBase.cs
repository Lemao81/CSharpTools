using System.Threading.Tasks;
using DockerConductor.Helpers;
using DockerConductor.Views;
using static DockerConductor.Helpers.DockerComposeCommandHelper;

namespace DockerConductor.Commands
{
    public abstract class BuildCommandExecutionBase : CommandExecutionBase
    {
        protected static async Task ExecuteBuildAsync(MainWindow window, string serviceName)
        {
            var basicCommand = GetBasicBuildCommand(window.ViewModel.BackendDockerComposePath, window.ViewModel.BackendDockerComposeOverridePath);
            var command      = Helper.ConcatCommand(basicCommand, serviceName);
            await Helper.ExecuteCliCommandAsync(command, window);
        }
    }
}
