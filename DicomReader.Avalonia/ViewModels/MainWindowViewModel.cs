using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Dtos;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IFileSystemService _fileSystemService;

        public MainWindowViewModel(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            DicomQueryViewModel = new DicomQueryViewModel();
            QueryResultViewModel = new QueryResultViewModel();
            PacsConfigurationViewModel = new PacsConfigurationViewModel();
        }

        public DicomQueryViewModel DicomQueryViewModel { get; }
        public QueryResultViewModel QueryResultViewModel { get; }
        public PacsConfigurationViewModel PacsConfigurationViewModel { get; }

        public void Initialize()
        {
            CreateConfigFileIfNotExist();
        }

        private void CreateConfigFileIfNotExist()
        {
            if (!_fileSystemService.FileExists(Consts.AppConfigFileName))
            {
                _fileSystemService.WriteFile(Consts.AppConfigFileName, CreateInitialAppConfig().AsIndentedJson());
            }
        }

        private static AppConfigDto CreateInitialAppConfig() => new(new AppConfig());
    }
}
