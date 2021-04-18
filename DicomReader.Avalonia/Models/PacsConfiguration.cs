using System;
using Common.Extensions;
using DicomReader.Avalonia.Dtos;
using DicomReader.Avalonia.ViewModels;

namespace DicomReader.Avalonia.Models
{
    public class PacsConfiguration
    {
        private PacsConfiguration()
        {
        }

        public PacsConfiguration(PacsConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            Name = configuration.Name;
            Host = configuration.Host;
            Port = configuration.Port;
            CallingAe = configuration.CallingAe;
            CalledAe = configuration.CalledAe;
        }

        public PacsConfiguration(PacsConfigurationViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            if (!int.TryParse(viewModel.Port, out var port)) throw new ArgumentException(nameof(viewModel.Port));

            Name = viewModel.Name;
            Host = viewModel.Host;
            Port = port;
            CallingAe = viewModel.CallingAe;
            CalledAe = viewModel.CalledAe;
        }

        public PacsConfiguration(PacsConfigurationDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (!int.TryParse(dto.Port, out var port)) throw new ArgumentException(nameof(dto.Port));

            Name = dto.Name ?? string.Empty;
            Host = dto.Host ?? string.Empty;
            Port = port;
            CallingAe = dto.CallingAe ?? string.Empty;
            CalledAe = dto.CalledAe ?? string.Empty;

            ValidatePacsConfiguration(this);
        }

        private static void ValidatePacsConfiguration(PacsConfiguration pacsConfiguration)
        {
            if (pacsConfiguration.Host.IsNullOrEmpty()) throw new InvalidOperationException("Host must not be empty");

            if (pacsConfiguration.Port <= 0) throw new InvalidOperationException("Port must be greater than 0");
        }

        public string Name { get; protected set; } = string.Empty;
        public string Host { get; protected set; } = string.Empty;
        public int Port { get; protected set; }
        public string CallingAe { get; protected set; } = string.Empty;
        public string CalledAe { get; protected set; } = string.Empty;
    }
}
