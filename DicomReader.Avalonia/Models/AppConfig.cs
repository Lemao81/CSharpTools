﻿using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Common.Extensions;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Dtos;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;

namespace DicomReader.Avalonia.Models
{
    public class AppConfig
    {
        private AppConfig()
        {
        }

        public AppConfig(AppConfigDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            LastLoadedPacsConfiguration = dto.LastLoadedPacsConfiguration ?? string.Empty;
            OutputFormat = dto.OutputFormat.IsNullOrEmpty() ? OutputFormat.None : Enum.Parse<OutputFormat>(dto.OutputFormat!);
            if (dto.PacsConfigurationDtos != null)
            {
                PacsConfigurations = new List<PacsConfiguration>(dto.PacsConfigurationDtos.Select(c => new PacsConfiguration(c)));
            }
        }

        public AppConfig(AppConfig appConfig, string lastLoaded)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            LastLoadedPacsConfiguration = lastLoaded;
            OutputFormat = appConfig.OutputFormat;
            PacsConfigurations.AddRange(appConfig.PacsConfigurations);
        }

        public AppConfig(AppConfig appConfig, OutputFormat outputFormat)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            LastLoadedPacsConfiguration = appConfig.LastLoadedPacsConfiguration;
            OutputFormat = outputFormat;
            PacsConfigurations.AddRange(appConfig.PacsConfigurations);
        }

        public static AppConfig Empty = new();

        public string LastLoadedPacsConfiguration { get; } = string.Empty;
        public OutputFormat OutputFormat { get; set; } = OutputFormat.JsonSerialized;
        public List<PacsConfiguration> PacsConfigurations { get; } = new();

        public void Save()
        {
            var fileSystemService = AvaloniaLocator.Current.GetService<IFileSystemService>();
            fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(this).AsIndentedJson());
        }
    }
}
