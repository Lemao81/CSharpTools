using DicomReader.WPF.Constants;
using DicomReader.WPF.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace DicomReader.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private string _title = "Dicom Reader";

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            ShowQueryPanelTabCommand = new DelegateCommand(ShowQueryPanelTab);
            ShowQueryResultTabCommand = new DelegateCommand(ShowQueryResultTab);
            ShowConfigurationTabCommand = new DelegateCommand(ShowConfigurationTab);

            ConfigurationTabUserControlViewModel.ConfigurationChanged += (s, e) => SelectedConfiguration = e.PacsConfiguration;
        }

        public static PacsConfiguration SelectedConfiguration { get; set; }

        #region bound properties
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        #endregion

        #region commands
        public DelegateCommand ShowQueryPanelTabCommand { get; }
        public DelegateCommand ShowQueryResultTabCommand { get; }
        public DelegateCommand ShowConfigurationTabCommand { get; }
        #endregion

        #region command delegates
        public void ShowQueryPanelTab()
        {
            _regionManager.RequestNavigate(Regions.Tab, "QueryPanelTabItem");
        }

        public void ShowQueryResultTab()
        {
            _regionManager.RequestNavigate(Regions.Tab, "QueryResultTabItem");
        }

        public void ShowConfigurationTab()
        {
            _regionManager.RequestNavigate(Regions.Tab, "ConfigurationTabItem");
        }
        #endregion
    }
}
