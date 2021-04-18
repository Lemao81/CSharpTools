using DicomReader.Avalonia.ViewModels;

namespace DicomReader.Avalonia.Helper
{
    public static class DesignData
    {
        public static MainWindowViewModel DesignMainViewModel { get; set; } = new MainWindowViewModel
        {
            Greeting = "From DesignData"
        };

        public static DicomQueryViewModel DesignDicomQueryViewModel { get; set; } = new DicomQueryViewModel
        {
        };

        public static QueryResultViewModel DesignQueryResultViewModel { get; set; } = new QueryResultViewModel
        {
        };

        public static PacsConfigurationViewModel DesignPacsConfigurationViewModel { get; set; } = new PacsConfigurationViewModel
        {
        };
    }
}
