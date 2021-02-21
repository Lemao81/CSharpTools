using DicomReader.WPF.Constants;
using DicomReader.WPF.Interfaces;
using DicomReader.WPF.Services;
using DicomReader.WPF.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DicomReader.WPF.Modules
{
    public class MainModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IFileSystemService, FileSystemService>();
            containerRegistry.Register<IDicomTagProvider, DicomTagProvider>();
            containerRegistry.Register<IDicomQueryService, DicomQueryService>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            // TODO change main tab name
            regionManager.RegisterViewWithRegion(Regions.Tab, typeof(QueryPanelTabUserControl));
            regionManager.RegisterViewWithRegion(Regions.Tab, typeof(QueryResultTabUserControl));
            regionManager.RegisterViewWithRegion(Regions.Tab, typeof(ConfigurationTabUserControl));
            regionManager.RegisterViewWithRegion(Regions.ResultTab, typeof(ListResultTabUserControl));
            regionManager.RegisterViewWithRegion(Regions.ResultTab, typeof(TreeResultTabUserControl));
        }
    }
}
