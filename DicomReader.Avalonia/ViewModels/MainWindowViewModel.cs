﻿using System;
using System.Linq;
using Avalonia;
using Common.Extensions;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Dtos;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DynamicData;

namespace DicomReader.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            DicomQueryViewModel = new DicomQueryViewModel();
            QueryResultViewModel = new QueryResultViewModel();
            PacsConfigurationViewModel = new PacsConfigurationViewModel();
            PacsConfigurationViewModel.SelectedConfigurationNameStream.Subscribe(configName =>
            {
                AppConfig = new AppConfig(AppConfig, configName);
                SaveAppConfig();
            });
            // TODO show log messages
            LogEntry.Stream.Subscribe(_ =>
            {
            });
        }

        public DicomQueryViewModel DicomQueryViewModel { get; }
        public QueryResultViewModel QueryResultViewModel { get; }
        public PacsConfigurationViewModel PacsConfigurationViewModel { get; }
        public AppConfig AppConfig { get; protected set; } = AppConfig.Empty;

        public void Initialize()
        {
            CreateConfigFileIfNotExist();
            InitializeAppConfig();
            AddSubscriptions();
            PacsConfigurationViewModel.Initialize(AppConfig);
        }

        private void CreateConfigFileIfNotExist()
        {
            var fileSystemService = AvaloniaLocator.Current.GetService<IFileSystemService>();
            if (!fileSystemService.FileExists(Consts.AppConfigFileName))
            {
                fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
            }
        }

        private void InitializeAppConfig()
        {
            var appConfigDto = AvaloniaLocator.Current.GetService<IFileSystemService>().ReadFile(Consts.AppConfigFileName).FromJson<AppConfigDto>();
            AppConfig = appConfigDto != null ? new AppConfig(appConfigDto) : AppConfig;
        }

        private void AddSubscriptions()
        {
            var dicomQueryService = AvaloniaLocator.Current.GetService<IDicomQueryService>();

            PacsConfigurationViewModel.SavePacsConfiguration?.Subscribe(editedConfiguration =>
            {
                AppConfig = new AppConfig(AppConfig, editedConfiguration.Name);
                var existingConfiguration = AppConfig.PacsConfigurations.SingleOrDefault(c => c.Name.EqualsIgnoringCase(editedConfiguration.Name));
                if (existingConfiguration != null)
                {
                    AppConfig.PacsConfigurations.Replace(existingConfiguration, editedConfiguration);
                }
                else
                {
                    AppConfig.PacsConfigurations.Add(editedConfiguration);
                }

                SaveAppConfig();
                PacsConfigurationViewModel.Initialize(AppConfig);
                PacsConfigurationViewModel.SelectedConfiguration = editedConfiguration;
            });

            DicomQueryViewModel.StartQuery?.Subscribe(async queryInputs =>
            {
                if (PacsConfigurationViewModel.SelectedConfiguration == null)
                    throw new InvalidOperationException("Dicom query started without selected pacs configuration");

                var resultSet = await dicomQueryService.ExecuteDicomQuery<DicomResultSet>(queryInputs, PacsConfigurationViewModel.SelectedConfiguration);
            });
        }

        private void SaveAppConfig()
        {
            var fileSystemService = AvaloniaLocator.Current.GetService<IFileSystemService>();
            fileSystemService.WriteFile(Consts.AppConfigFileName, new AppConfigDto(AppConfig).AsIndentedJson());
        }
    }
}
