using DockerConductor.ViewModels;

namespace DockerConductor.Models
{
    public class AppConfig
    {
        public string DockerComposePath         { get; set; } = string.Empty;
        public string DockerComposeOverridePath { get; set; } = string.Empty;
        public string Excludes                  { get; set; } = string.Empty;
        public string ThirdParties              { get; set; } = string.Empty;
        public string Usuals                    { get; set; } = string.Empty;
        public string FirstBatch                { get; set; } = string.Empty;
        public string FirstBatchWait            { get; set; } = "10";
        public string SecondBatch               { get; set; } = string.Empty;
        public string SecondBatchWait           { get; set; } = "10";
        public string DbVolume                  { get; set; } = string.Empty;

        public void MapInto(MainWindowViewModel viewModel)
        {
            viewModel.DockerComposePath         = DockerComposePath;
            viewModel.DockerComposeOverridePath = DockerComposeOverridePath;
            viewModel.Excludes                  = Excludes;
            viewModel.ThirdParties              = ThirdParties;
            viewModel.Usuals                    = Usuals;
            viewModel.FirstBatch                = FirstBatch;
            viewModel.FirstBatchWait            = FirstBatchWait;
            viewModel.SecondBatch               = SecondBatch;
            viewModel.SecondBatchWait           = SecondBatchWait;
            viewModel.DbVolume                  = DbVolume;
        }

        public void MapFrom(MainWindowViewModel viewModel)
        {
            DockerComposePath         = viewModel.DockerComposePath;
            DockerComposeOverridePath = viewModel.DockerComposeOverridePath;
            Excludes                  = viewModel.Excludes;
            ThirdParties              = viewModel.ThirdParties;
            Usuals                    = viewModel.Usuals;
            FirstBatch                = viewModel.FirstBatch;
            FirstBatchWait            = viewModel.FirstBatchWait;
            SecondBatch               = viewModel.SecondBatch;
            SecondBatchWait           = viewModel.SecondBatchWait;
            DbVolume                  = viewModel.DbVolume;
        }
    }
}
