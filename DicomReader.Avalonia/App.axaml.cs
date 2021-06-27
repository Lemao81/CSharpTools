using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DicomReader.Avalonia.Factories;
using DicomReader.Avalonia.Handler;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.Services;
using DicomReader.Avalonia.Strategies;
using DicomReader.Avalonia.ViewModels;
using DicomReader.Avalonia.Views;

namespace DicomReader.Avalonia
{
    public class App : Application
    {
        public static bool IsExtendedLog { get; set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                RegisterAppServices();

                var mainWindowViewModel = new MainWindowViewModel();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel,
                };
                mainWindowViewModel.Initialize();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void RegisterAppServices()
        {
            AvaloniaLocator.CurrentMutable.Bind<IFileSystemService>().ToSingleton<FileSystemService>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomCFindRequestFactoryProvider>().ToSingleton<DicomCFindRequestFactoryProvider>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomQueryService>().ToSingleton<DicomQueryService>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomResponseProcessingStrategy<DicomResultSet>>().ToSingleton<DicomResultSetResponseProcessingStrategy>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomResponseProcessingStrategy<string>>().ToSingleton<SerializedJsonDicomResponseProcessingStrategy>();
            AvaloniaLocator.CurrentMutable.Bind<IStartQueryHandler>().ToSingleton<StartQueryHandler>();
            AvaloniaLocator.CurrentMutable.Bind<ISavePacsConfigurationHandler>().ToSingleton<SavePacsConfigurationHandler>();
            AvaloniaLocator.CurrentMutable.Bind<IConfigurationChangedHandler>().ToSingleton<ConfigurationChangedHandler>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomTagProvider>().ToSingleton<DicomTagProvider>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomRequestFactory>().ToSingleton<DicomRequestFactory>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomCMoveRequestFactory>().ToSingleton<DicomCMoveRequestFactory>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomRequestFactoryProvider>().ToSingleton<DicomRequestFactoryProvider>();
            AvaloniaLocator.CurrentMutable.Bind<IDicomClientFactory>().ToSingleton<DicomClientFactory>();
        }
    }
}
