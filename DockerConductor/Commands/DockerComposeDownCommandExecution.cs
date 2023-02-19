using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Helpers;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class DockerComposeDownCommandExecution : CommandExecutionBase
    {
        public static DockerComposeDownCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            var command = Helper.ConcatCommand(
                Consts.DockerCompose,
                Helper.ConcatFilePathArguments(window.ViewModel.BackendDockerComposePath, window.ViewModel.BackendDockerComposeOverridePath),
                "down"
            );

            await Helper.ExecuteCliCommandAsync(command, window);
        }
    }
}
