using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;
using DynamicData;

namespace DicomReader.Avalonia.Helper
{
    public static class DesignData
    {
        public static MainWindowViewModel DesignMainViewModel { get; set; } = new();

        public static DicomQueryViewModel DesignDicomQueryViewModel { get; set; } = new();

        public static QueryResultViewModel DesignQueryResultViewModel { get; set; } = new();

        public static ConfigurationViewModel DesignConfigurationViewModel { get; set; } = GetDesignPacsConfigurationViewModel();

        private static ConfigurationViewModel GetDesignPacsConfigurationViewModel()
        {
            var viewModel = new ConfigurationViewModel
            {
                Host      = "192.168.35.50",
                Port      = "4242",
                CallingAe = "RRNEOQ",
                CalledAe  = "ORTHANC",
                ScpPort   = "0"
            };

            viewModel.PacsConfigurations.AddRange(new[] { new PacsConfiguration(viewModel) });

            return viewModel;
        }
    }
}
