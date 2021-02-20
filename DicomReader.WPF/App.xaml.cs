using DicomReader.WPF.Views;
using Prism.Ioc;
using System.Windows;
using System.Windows.Controls;
using DicomReader.WPF.Interfaces;
using DicomReader.WPF.Modules;
using DicomReader.WPF.RegionAdapters;
using DicomReader.WPF.Services;
using Prism.Modularity;
using Prism.Regions;

namespace DicomReader.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var baseCatalog = base.CreateModuleCatalog();
            baseCatalog.AddModule<MainModule>();

            return baseCatalog;
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping<TabControl>(Container.Resolve<TabControlAdapter>());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
