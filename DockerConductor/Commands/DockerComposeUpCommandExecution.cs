using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Helpers;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class DockerComposeUpCommandExecution : CommandExecutionBase
    {
        public static DockerComposeUpCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            var basicCommand = Helper.ConcatCommand(
                Consts.DockerCompose,
                Helper.ConcatFilePathArguments(window.ViewModel.BackendDockerComposePath, window.ViewModel.BackendDockerComposeOverridePath),
                "up",
                "-d",
                "--no-deps"
            );

            await Helper.ExecuteCliCommandAsync(Helper.ConcatCommand(basicCommand, window.ViewModel.SelectedServiceNames), window);
        }
    }
}
