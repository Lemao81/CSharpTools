using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DockerConductor.Constants;
using DockerConductor.Extensions;
using DockerConductor.Views;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DockerConductor.Commands
{
    public class BuildTraefikCommandExecution : BuildCommandExecutionBase
    {
        private static readonly ISerializer YamlSerializer = new SerializerBuilder().WithDefaultScalarStyle(ScalarStyle.DoubleQuoted)
                                                                                    .WithIndentedSequences()
                                                                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                                                    .Build();

        public static BuildTraefikCommandExecution Instance = new();

        protected override async Task DoExecuteAsync(MainWindow window)
        {
            await ReplaceAndSaveTraefikAsync(window);
            window.TabControl.SwitchToTab(TabItemIndex.Panel);
            await ExecuteBuildAsync(window, ServiceNames.Traefik);
        }

        private async Task ReplaceAndSaveTraefikAsync(MainWindow window)
        {
            if (window.ViewModel.TraefikServicesHttp?.Http?.Services is null) return;

            foreach (var routeUi in window.TraefikRouteUis)
            {
                window.ViewModel.TraefikServicesHttp.Http.Services[routeUi.Name].LoadBalancer.Servers.First().Url =
                    routeUi.IsInternalHost ? $"http://host.docker.internal:{routeUi.Port}" : routeUi.OrigHost;
            }

            var yamlText = YamlSerializer.Serialize(window.ViewModel.TraefikServicesHttp);
            foreach (var routeUi in window.TraefikRouteUis)
            {
                yamlText = yamlText.Replace($"\"{routeUi.Name}\"", routeUi.Name);
            }

            await File.WriteAllTextAsync(window.ViewModel.TraefikServicesPath, yamlText);
        }
    }
}
