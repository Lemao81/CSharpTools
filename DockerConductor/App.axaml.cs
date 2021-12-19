using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DockerConductor.Helpers;
using DockerConductor.Models;
using DockerConductor.ViewModels;
using DockerConductor.Views;
using Newtonsoft.Json;

namespace DockerConductor
{
    public class App : Application
    {
        public const string ConfigFileName = "config.json";

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;
                var mainWindowViewModel = new MainWindowViewModel(desktop.MainWindow);
                desktop.MainWindow.DataContext = mainWindowViewModel;

                AppConfig appConfig;
#if DEVELOP
                if (File.Exists(ConfigFileName))
                {
                    File.Delete(ConfigFileName);
                }
#endif
                if (!File.Exists(ConfigFileName))
                {
                    appConfig = new AppConfig();

                    File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(appConfig, Formatting.Indented));
                }
                else
                {
                    appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(ConfigFileName))!;
                }

                mainWindowViewModel.DockerComposePath         = appConfig.DockerComposePath;
                mainWindowViewModel.DockerComposeOverridePath = appConfig.DockerComposeOverridePath;
                mainWindowViewModel.Excludes                  = appConfig.Excludes;
                mainWindowViewModel.FirstBatch                = appConfig.FirstBatch;
                mainWindowViewModel.FirstBatchWait            = appConfig.FirstBatchWait;
                mainWindowViewModel.SecondBatch               = appConfig.SecondBatch;
                mainWindowViewModel.SecondBatchWait           = appConfig.SecondBatchWait;

                if (!string.IsNullOrWhiteSpace(appConfig.DockerComposePath))
                {
                    Helper.UpdateServiceCheckboxList(mainWindow);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
