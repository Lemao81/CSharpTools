using System.Threading.Tasks;
using DockerConductor.Views;

namespace DockerConductor.Commands
{
    public abstract class CommandExecutionBase
    {
        public async Task ExecuteAsync(MainWindow window)
        {
            window.ViewModel.SwitchBusy();
            await DoExecuteAsync(window);
            window.ViewModel.SwitchIdle();
        }

        protected abstract Task DoExecuteAsync(MainWindow window);
    }
}
