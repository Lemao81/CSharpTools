using DicomReader.WPF.Extensions;
using DicomReader.WPF.ViewModels;

namespace DicomReader.WPF.Models
{
    public class PacsConfiguration
    {
        private PacsConfiguration()
        {
        }

        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string CallingAet { get; set; }
        public string CalledAet { get; set; }

        public static Result<PacsConfiguration> Create(string name, string host, string port, string callingAet, string calledAet)
        {
            if (name.IsNullOrEmpty() || host.IsNullOrEmpty() || !int.TryParse(port, out var portNumber))
            {
                return Result<PacsConfiguration>.Fail();
            }

            return Result<PacsConfiguration>.Success(new PacsConfiguration
            {
                Name = name,
                Host = host,
                Port = portNumber,
                CallingAet = callingAet ?? string.Empty,
                CalledAet = calledAet ?? string.Empty
            });
        }

        public static Result<PacsConfiguration> Create(ConfigurationTabUserControlViewModel viewModel) =>
            Create(viewModel.ConfigurationName, viewModel.Host, viewModel.Port, viewModel.CallingAet, viewModel.CalledAet);
    }
}
