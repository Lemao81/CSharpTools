using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;
using DynamicData;

namespace DicomReader.Avalonia.Helper
{
    public static class DesignData
    {
        public static MainWindowViewModel DesignMainViewModel { get; set; } = new MainWindowViewModel();

        public static DicomQueryViewModel DesignDicomQueryViewModel { get; set; } = new DicomQueryViewModel();

        public static QueryResultViewModel DesignQueryResultViewModel { get; set; } = new QueryResultViewModel();

        public static ConfigurationViewModel DesignConfigurationViewModel { get; set; } = GetDesignPacsConfigurationViewModel();

        private static ConfigurationViewModel GetDesignPacsConfigurationViewModel()
        {
            var viewModel = new ConfigurationViewModel
            {
                Host = "192.168.35.50",
                Port = "4242",
                CallingAe = "RRNEOQ",
                CalledAe = "ORTHANC"
            };
            viewModel.PacsConfigurations.AddRange(new[] { new PacsConfiguration(viewModel) });

            return viewModel;
        }
    }
}
