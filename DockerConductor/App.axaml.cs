using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DockerConductor.Models;
using DockerConductor.ViewModels;
using DockerConductor.Views;
using Newtonsoft.Json;
using YamlDotNet.Serialization.NamingConventions;

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
                desktop.MainWindow = new MainWindow();
                var mainWindowViewModel = new MainWindowViewModel(desktop.MainWindow);
                desktop.MainWindow.DataContext = mainWindowViewModel;

                AppConfig appConfig;
                if (!File.Exists(ConfigFileName))
                {
                    appConfig = new AppConfig();

                    File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(appConfig, Formatting.Indented));
                }
                else
                {
                    appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(ConfigFileName))!;
                }

                mainWindowViewModel.DockerComposePath = appConfig.DockerComposePath;
                mainWindowViewModel.Excludes          = appConfig.Excludes;
                mainWindowViewModel.FirstBatch        = appConfig.FirstBatch;
                mainWindowViewModel.SecondBatch       = appConfig.SecondBatch;

                if (!string.IsNullOrWhiteSpace(appConfig.DockerComposePath))
                {
                    AddServiceSelections(appConfig.DockerComposePath, desktop.MainWindow);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static void AddServiceSelections(string dockerComposePath, Window window)
        {
            var dockerComposeText = File.ReadAllText(dockerComposePath);
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().IgnoreUnmatchedProperties()
                                                                                 .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                                                                 .Build();

            var dockerCompose = deserializer.Deserialize<DockerCompose>(dockerComposeText);

            var mainWindow                = window as MainWindow;
            var serviceSelectionContainer = mainWindow?.ServiceSelectionContainer;
            if (serviceSelectionContainer != null)
            {
                serviceSelectionContainer.Children.Clear();
                foreach (var serviceName in dockerCompose.Services.Keys)
                {
                    var checkBox = new CheckBox { Content = serviceName };
                    serviceSelectionContainer.Children.Add(checkBox);
                }
            }
        }
    }
}
