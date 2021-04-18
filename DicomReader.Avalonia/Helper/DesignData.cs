using DicomReader.Avalonia.Services;
using DicomReader.Avalonia.ViewModels;
using DynamicData;

namespace DicomReader.Avalonia.Helper
{
    public static class DesignData
    {
        public static MainWindowViewModel DesignMainViewModel { get; set; } = new MainWindowViewModel(new FileSystemService());

        public static DicomQueryViewModel DesignDicomQueryViewModel { get; set; } = new DicomQueryViewModel();

        public static QueryResultViewModel DesignQueryResultViewModel { get; set; } = new QueryResultViewModel();

        public static PacsConfigurationViewModel DesignPacsConfigurationViewModel { get; set; } = GetDesignPacsConfigurationViewModel();

        private static PacsConfigurationViewModel GetDesignPacsConfigurationViewModel()
        {
            var viewModel = new PacsConfigurationViewModel
            {
                Host = "192.168.35.50",
                Port = "4242",
                CallingAe = "RRNEOQ",
                CalledAe = "ORTHANC"
            };
            viewModel.PacsConfigurationNames.AddRange(new[] { "Staging", "Med360", "Potsdam" });

            return viewModel;
        }
    }
}
