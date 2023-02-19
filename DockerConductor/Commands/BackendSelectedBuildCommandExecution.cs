using System.Linq;
using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Extensions;
using DockerConductor.Helpers;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class BackendSelectedBuildCommandExecution : CommandExecutionBase
    {
        public static BackendSelectedBuildCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            var selectedBuildNames = window.ViewModel.SelectedBuildNames.ToArray();
            if (!selectedBuildNames.Any()) return;

            foreach (var button in window.BuildSelectionToggleButtons)
            {
                button.UnCheck();
            }

            var basicCommand = Helper.ConcatCommand(
                Consts.DockerCompose,
                Helper.ConcatFilePathArguments(window.ViewModel.BackendDockerComposePath, window.ViewModel.BackendDockerComposeOverridePath),
                "build"
            );

            await Helper.ExecuteCliCommandAsync(Helper.ConcatCommand(basicCommand, selectedBuildNames), window);
        }
    }
}
