using DicomReader.WPF.Extensions;
using DicomReader.WPF.ViewModels;

namespace DicomReader.WPF.Models
{
    public class PacsConfiguration
    {
        private PacsConfiguration()
        {
        }

        public string Host { get; protected set; }
        public int Port { get; protected set; }
        public string CallingAet { get; protected set; }
        public string CalledAet { get; protected set; }

        public static Result<PacsConfiguration> Create(string host, string port, string callingAet, string calledAet)
        {
            if (host.IsNullOrEmpty() || !int.TryParse(port, out var portNumber))
            {
                return Result<PacsConfiguration>.Fail();
            }

            return Result<PacsConfiguration>.Success(new PacsConfiguration
            {
                Host = host,
                Port = portNumber,
                CallingAet = callingAet ?? string.Empty,
                CalledAet = calledAet ?? string.Empty
            });
        }

        public static Result<PacsConfiguration> Create(ConfigurationTabUserControlViewModel viewModel) =>
            Create(viewModel.Host, viewModel.Port, viewModel.CallingAet, viewModel.CalledAet);
    }
}
