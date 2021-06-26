using System;
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
    class StartQueryHandler : IStartQueryHandler
    {
        public async Task StartQueryAsync(MainWindowViewModel mainWindowViewModel, DicomQueryInputs queryInputs)
        {
            var dicomQueryService = AvaloniaLocator.Current.GetService<IDicomQueryService>();
            var configViewModel = mainWindowViewModel.ConfigurationViewModel;
            var resultViewModel = mainWindowViewModel.QueryResultViewModel;
            var queryViewModel = mainWindowViewModel.DicomQueryViewModel;
            var appConfig = mainWindowViewModel.AppConfig;

            if (configViewModel.SelectedConfiguration == null)
                throw new InvalidOperationException("Dicom query started without selected pacs configuration");

            resultViewModel.Json = string.Empty;
            resultViewModel.Results.Clear();
            resultViewModel.FlattenedResults.Clear();

            IDicomResponseCollector responseCollector = queryInputs.PagedQueryParams.IsPaged && queryInputs.PagedQueryParams.PageSize.HasValue
                ? new PagedDicomResponseCollector(queryInputs.PagedQueryParams.PageSize.Value, queryInputs.PagedQueryParams.Page)
                : new UnPagedDicomResponseCollector();

            switch (appConfig.OutputFormat)
            {
                case OutputFormat.JsonSerialized:
                    var serializedString =
                        await dicomQueryService.ExecuteDicomQuery<string>(queryInputs, configViewModel.SelectedConfiguration,
                            responseCollector);
                    resultViewModel.Json = serializedString;
                    break;
                case OutputFormat.DicomResult:
                    var resultSet = await dicomQueryService.ExecuteDicomQuery<DicomResultSet>(queryInputs,
                        configViewModel.SelectedConfiguration,
                        responseCollector);
                    foreach (var result in resultSet.Results)
                    {
                        result.RemoveAll(r => queryViewModel.RequestedDicomTags.All(t => r.Keyword != t.Name && r.HexCode != t.HexCode));
                    }
                    resultViewModel.Json = resultSet.AsIndentedJson();
                    resultViewModel.Results.AddRange(resultSet.Results);
                    resultViewModel.FlattenedResults.AddRange(resultSet.Results.SelectMany(r => r));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(queryInputs));
            }

            mainWindowViewModel.MainViewTab = MainViewTab.Result;
            resultViewModel.ResultTab = ResultTab.Json;
        }
    }
}
