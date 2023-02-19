using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Helpers;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class DockerBuildAllCommandExecution : CommandExecutionBase
    {
        public static DockerBuildAllCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            var command = Helper.ConcatCommand(
                Consts.DockerCompose,
                Helper.ConcatFilePathArguments(window.ViewModel.BackendDockerComposePath, window.ViewModel.BackendDockerComposeOverridePath),
                "build"
            );

            await Helper.ExecuteCliCommandAsync(command, window);
        }
    }
}
