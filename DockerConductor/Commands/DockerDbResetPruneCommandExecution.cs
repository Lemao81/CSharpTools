using System.Threading.Tasks;
using DockerConductor.Helpers;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public class DockerDbResetPruneCommandExecution : CommandExecutionBase
    {
        public static DockerDbResetPruneCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            if (string.IsNullOrWhiteSpace(window.ViewModel.DbVolume)) return;

            var command = $"docker volume rm {window.ViewModel.DbVolume} && docker system prune -f";
            await Helper.ExecuteCliCommandAsync(command, window);
        }
    }
}
