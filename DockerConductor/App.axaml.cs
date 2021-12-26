using System.IO;
using System.Linq;
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

                appConfig.MapInto(mainWindowViewModel);

                if (!string.IsNullOrWhiteSpace(appConfig.DockerComposePath))
                {
                    Helper.UpdateServiceCheckboxList(mainWindow);
                }

                if (!string.IsNullOrWhiteSpace(appConfig.OcelotConfigurationPath))
                {
                    Helper.UpdateOcelotItemList(mainWindow);
                }

                if (appConfig.LastSelected.Any())
                {
                    foreach (var checkBox in mainWindow.ServiceSelectionCheckBoxes.Where(c => appConfig.LastSelected.Contains(c.Content.ToString())))
                    {
                        checkBox.IsChecked = true;
                    }
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
