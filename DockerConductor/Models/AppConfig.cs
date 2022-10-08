using System.Collections.Generic;
using System.Linq;
using DockerConductor.ViewModels;

namespace DockerConductor.Models
{
    public class AppConfig
    {
        public string              BackendRepoPath         { get; set; } = string.Empty;
        public string              FrontendRepoPath        { get; set; } = string.Empty;
        public string              OcelotConfigurationPath { get; set; } = string.Empty;
        public string              Excludes                { get; set; } = string.Empty;
        public string              ThirdParties            { get; set; } = string.Empty;
        public string              Usuals                  { get; set; } = string.Empty;
        public string              FirstBatch              { get; set; } = string.Empty;
        public string              FirstBatchWait          { get; set; } = "10";
        public string              SecondBatch             { get; set; } = string.Empty;
        public string              SecondBatchWait         { get; set; } = "10";
        public string              ExcludesStop            { get; set; } = string.Empty;
        public string              DbVolume                { get; set; } = string.Empty;
        public string              DevServerIp             { get; set; } = string.Empty;
        public IEnumerable<string> LastSelected            { get; set; } = Enumerable.Empty<string>();

        public void MapInto(MainWindowViewModel viewModel)
        {
            viewModel.BackendRepoPath         = BackendRepoPath;
            viewModel.FrontendRepoPath        = FrontendRepoPath;
            viewModel.OcelotConfigurationPath = OcelotConfigurationPath;
            viewModel.Excludes                = Excludes;
            viewModel.ThirdParties            = ThirdParties;
            viewModel.Usuals                  = Usuals;
            viewModel.FirstBatch              = FirstBatch;
            viewModel.FirstBatchWait          = FirstBatchWait;
            viewModel.SecondBatch             = SecondBatch;
            viewModel.SecondBatchWait         = SecondBatchWait;
            viewModel.ExcludesStop            = ExcludesStop;
            viewModel.DbVolume                = DbVolume;
            viewModel.DevServerIp             = DevServerIp;
            viewModel.LastSelected            = LastSelected;
        }

        public void MapFrom(MainWindowViewModel viewModel)
        {
            BackendRepoPath         = viewModel.BackendRepoPath;
            FrontendRepoPath        = viewModel.FrontendRepoPath;
            OcelotConfigurationPath = viewModel.OcelotConfigurationPath;
            Excludes                = viewModel.Excludes;
            ThirdParties            = viewModel.ThirdParties;
            Usuals                  = viewModel.Usuals;
            FirstBatch              = viewModel.FirstBatch;
            FirstBatchWait          = viewModel.FirstBatchWait;
            SecondBatch             = viewModel.SecondBatch;
            SecondBatchWait         = viewModel.SecondBatchWait;
            ExcludesStop            = viewModel.ExcludesStop;
            DbVolume                = viewModel.DbVolume;
            DevServerIp             = viewModel.DevServerIp;
            LastSelected            = viewModel.LastSelected;
        }
    }
}
