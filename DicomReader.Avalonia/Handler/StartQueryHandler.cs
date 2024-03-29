﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.Services;
using DicomReader.Avalonia.ViewModels;
using DynamicData;

namespace DicomReader.Avalonia.Handler
{
    public class StartQueryHandler : IStartQueryHandler
    {
        private readonly IDicomQueryService _dicomQueryService;

        public StartQueryHandler()
        {
            _dicomQueryService = AvaloniaLocator.Current.GetService<IDicomQueryService>();
        }

        public async Task StartQueryAsync(MainWindowViewModel mainWindowViewModel, DicomQueryInputs queryInputs)
        {
            var configViewModel = mainWindowViewModel.ConfigurationViewModel;
            var resultViewModel = mainWindowViewModel.QueryResultViewModel;
            var queryViewModel  = mainWindowViewModel.DicomQueryViewModel;
            var appConfig       = mainWindowViewModel.AppConfig;

            if (configViewModel.SelectedConfiguration == null) throw new InvalidOperationException("Dicom query started without selected pacs configuration");

            ClearCurrentResult(resultViewModel);
            var responseCollector = GetDicomResponseCollector(queryInputs);

            switch (appConfig.OutputFormat)
            {
                case OutputFormat.JsonSerialized:
                    await ExecuteWithJsonSerializedResult(queryInputs, _dicomQueryService, configViewModel, responseCollector, resultViewModel);
                    break;
                case OutputFormat.DicomResult:
                    await ExecuteWithDicomResult(queryInputs, _dicomQueryService, configViewModel, responseCollector, queryViewModel, resultViewModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(queryInputs));
            }

            mainWindowViewModel.MainViewTab = MainViewTab.Result;
            resultViewModel.ResultTab       = ResultTab.Json;
        }

        private static void ClearCurrentResult(QueryResultViewModel resultViewModel)
        {
            resultViewModel.Json = string.Empty;
            resultViewModel.Results.Clear();
            resultViewModel.FlattenedResults.Clear();
        }

        private static async Task ExecuteWithJsonSerializedResult(
            DicomQueryInputs        queryInputs,
            IDicomQueryService      dicomQueryService,
            ConfigurationViewModel  configViewModel,
            IDicomResponseCollector responseCollector,
            QueryResultViewModel    resultViewModel)
        {
            var serializedString = await dicomQueryService.ExecuteDicomQuery<string>(queryInputs, configViewModel.SelectedConfiguration, responseCollector);

            resultViewModel.Json = serializedString;
        }

        private static async Task ExecuteWithDicomResult(
            DicomQueryInputs        queryInputs,
            IDicomQueryService      dicomQueryService,
            ConfigurationViewModel  configViewModel,
            IDicomResponseCollector responseCollector,
            DicomQueryViewModel     queryViewModel,
            QueryResultViewModel    resultViewModel)
        {
            var resultSet = await dicomQueryService.ExecuteDicomQuery<DicomResultSet>(queryInputs, configViewModel.SelectedConfiguration, responseCollector);
            foreach (var result in resultSet.Results)
            {
                result.RemoveAll(r => queryViewModel.RequestedDicomTags.All(t => r.Keyword != t.Name && r.HexCode != t.HexCode));
            }

            resultViewModel.Json = resultSet.AsIndentedJson();
            resultViewModel.Results.AddRange(resultSet.Results);
            resultViewModel.FlattenedResults.AddRange(resultSet.Results.SelectMany(r => r));
        }

        private static IDicomResponseCollector GetDicomResponseCollector(DicomQueryInputs queryInputs) =>
            queryInputs.PagedQueryParams.IsPaged && queryInputs.PagedQueryParams.PageSize.HasValue
                ? new PagedDicomResponseCollector(queryInputs.PagedQueryParams.PageSize.Value, queryInputs.PagedQueryParams.Page)
                : new UnPagedDicomResponseCollector();
    }
}
