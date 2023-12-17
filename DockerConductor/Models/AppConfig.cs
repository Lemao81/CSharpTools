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
        public string              UsersSetup              { get; set; } = string.Empty;
        public string              StartUsuals             { get; set; } = string.Empty;
        public string              BuildUsuals             { get; set; } = string.Empty;
        public string              FirstBatch              { get; set; } = string.Empty;
        public string              FirstBatchWait          { get; set; } = "10";
        public string              SecondBatch             { get; set; } = string.Empty;
        public string              SecondBatchWait         { get; set; } = "10";
        public string              ExcludesStop            { get; set; } = string.Empty;
        public string              DbVolume                { get; set; } = string.Empty;
        public string              DevServerIp             { get; set; } = string.Empty;
        public IEnumerable<string> LastSelected            { get; set; } = Enumerable.Empty<string>();
        public string              TraefikServicesPath     { get; set; } = string.Empty;

        public void MapInto(MainWindowViewModel viewModel)
        {
            viewModel.BackendRepoPath         = BackendRepoPath;
            viewModel.FrontendRepoPath        = FrontendRepoPath;
            viewModel.OcelotConfigurationPath = OcelotConfigurationPath;
            viewModel.Excludes                = Excludes;
            viewModel.ThirdParties            = ThirdParties;
            viewModel.UsersSetup              = UsersSetup;
            viewModel.StartUsuals             = StartUsuals;
            viewModel.BuildUsuals             = BuildUsuals;
            viewModel.FirstBatch              = FirstBatch;
            viewModel.FirstBatchWait          = FirstBatchWait;
            viewModel.SecondBatch             = SecondBatch;
            viewModel.SecondBatchWait         = SecondBatchWait;
            viewModel.ExcludesStop            = ExcludesStop;
            viewModel.DbVolume                = DbVolume;
            viewModel.DevServerIp             = DevServerIp;
            viewModel.LastSelected            = LastSelected;
            viewModel.TraefikServicesPath     = TraefikServicesPath;
        }

        public void MapFrom(MainWindowViewModel viewModel)
        {
            BackendRepoPath         = viewModel.BackendRepoPath;
            FrontendRepoPath        = viewModel.FrontendRepoPath;
            OcelotConfigurationPath = viewModel.OcelotConfigurationPath;
            Excludes                = viewModel.Excludes;
            ThirdParties            = viewModel.ThirdParties;
            UsersSetup              = viewModel.UsersSetup;
            StartUsuals             = viewModel.StartUsuals;
            BuildUsuals             = viewModel.BuildUsuals;
            FirstBatch              = viewModel.FirstBatch;
            FirstBatchWait          = viewModel.FirstBatchWait;
            SecondBatch             = viewModel.SecondBatch;
            SecondBatchWait         = viewModel.SecondBatchWait;
            ExcludesStop            = viewModel.ExcludesStop;
            DbVolume                = viewModel.DbVolume;
            DevServerIp             = viewModel.DevServerIp;
            LastSelected            = viewModel.LastSelected;
            TraefikServicesPath     = viewModel.TraefikServicesPath;
        }
    }
}
