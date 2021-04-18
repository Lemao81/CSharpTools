using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DicomReader.Avalonia.Services;
using DicomReader.Avalonia.ViewModels;
using DicomReader.Avalonia.Views;

namespace DicomReader.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindowViewModel = new MainWindowViewModel(new FileSystemService());
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel,
                };
                mainWindowViewModel.Initialize();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
