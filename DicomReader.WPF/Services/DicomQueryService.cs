﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Dicom;
using Dicom.Network;
using DicomReader.WPF.Extensions;
using DicomReader.WPF.Helpers;
using DicomReader.WPF.Interfaces;
using DicomReader.WPF.Models;
using DicomReader.WPF.ViewModels;
using static System.String;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.WPF.Services
{
    public class DicomQueryService : IDicomQueryService
    {
        private readonly IDicomTagProvider _dicomTagProvider;

        public DicomQueryService(IDicomTagProvider dicomTagProvider)
        {
            _dicomTagProvider = dicomTagProvider;
        }

        public async Task<List<DicomResultSet>> ExecuteDicomQuery(QueryPanelTabUserControlViewModel viewModel, PacsConfiguration pacsConfiguration)
        {
            var findRequest = CreateFindRequest(viewModel);

            return await SendFindRequestAndCollectResult(findRequest, pacsConfiguration);
        }

        private DicomCFindRequest CreateFindRequest(QueryPanelTabUserControlViewModel viewModel)
        {
            var findRequest = new DicomCFindRequest(viewModel.RetrieveLevel);

            switch (viewModel.RetrieveLevel)
            {
                case DicomQueryRetrieveLevel.Patient:
                    findRequest.Dataset.AddOrUpdate(DicomTag.PatientID, viewModel.PatientId ?? Empty);
                    break;
                case DicomQueryRetrieveLevel.Study:
                    findRequest.Dataset.AddOrUpdate(DicomTag.PatientID, viewModel.PatientId ?? Empty);
                    findRequest.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, viewModel.StudyInstanceUid ?? Empty);
                    findRequest.Dataset.AddOrUpdate(DicomTag.AccessionNumber, viewModel.AccessionNumber ?? Empty);
                    break;
                case DicomQueryRetrieveLevel.Series:
                    findRequest.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, viewModel.StudyInstanceUid ?? Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var keywordOrHexCode in viewModel.RequestedFields.Select(r => r.Trim()).Where(r => !r.IsNullOrEmpty()))
            {
                _dicomTagProvider.ProvideDicomTag(keywordOrHexCode)
                    .OnSuccess(dicomTag => AddDicomTagToRequestIfNotExist(findRequest, dicomTag))
                    .OnException(exception => throw exception);
            }

            return findRequest;
        }

        private static void AddDicomTagToRequestIfNotExist(DicomCFindRequest findRequest, DicomTag tag)
        {
            if (findRequest.Dataset.Contains(tag)) return;

            var accordingEntry = DicomDictionary.Default.FirstOrDefault(_ => _.Tag == tag);
            if (accordingEntry == null) return;

            var vr = accordingEntry.ValueRepresentations.First();
            if (vr.ValueType == null) return;

            var addMethod = typeof(DicomQueryService).GetMethod(nameof(DatasetAddGeneric), BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(vr.ValueType);
            addMethod?.Invoke(null, new object[] { findRequest, tag });
        }

        private static void DatasetAddGeneric<T>(DicomCFindRequest findRequest, DicomTag tag) => findRequest.Dataset.AddOrUpdate<T>(tag);

        private static async Task<List<DicomResultSet>> SendFindRequestAndCollectResult(DicomCFindRequest findRequest, PacsConfiguration pacsConfiguration)
        {
            try
            {
                var client = new DicomClient(pacsConfiguration.Host, pacsConfiguration.Port, false,
                    pacsConfiguration.CallingAet.IsNullOrEmpty() ? "FINDSCU" : pacsConfiguration.CallingAet,
                    pacsConfiguration.CalledAet);
                client.NegotiateAsyncOps();

                var result = new List<DicomResultSet>();
                findRequest.OnResponseReceived = (_, response) =>
                {
                    Console.WriteLine("findRequest.OnResponseReceived");
                    if (!response.HasDataset || response.Dataset == null) return;
                    Console.WriteLine("findRequest.OnResponseReceived - with data");

                    var dataSetValues = new List<DicomResult>();
                    dataSetValues.AddRange(response.Dataset.Select(entry =>
                        new DicomResult
                        {
                            Name = entry.Tag.DictionaryEntry.Name,
                            Keyword = entry.Tag.DictionaryEntry.Keyword,
                            HexCode = $"{entry.Tag.Group:X4}:{entry.Tag.Element:X4}",
                            ValueRepresentation = entry.Tag.DictionaryEntry.ValueRepresentations.FirstOrDefault()?.Name ?? Empty,
                            StringValue = response.Dataset.GetString(entry.Tag)
                        }));
                    result.Add(new DicomResultSet(dataSetValues));
                };
                findRequest.OnTimeout = (_, args) =>
                {
                    Console.WriteLine("findRequest.OnTimeout");
                };
                client.StateChanged += (sender, args) =>
                {
                    Console.WriteLine($"State: {args.NewState}");
                };
                client.AssociationAccepted += (sender, args) =>
                {
                    Console.WriteLine("client.AssociationAccepted");
                };
                client.AssociationRejected += (sender, args) =>
                {
                    Console.WriteLine("client.AssociationRejected");
                    throw new ApplicationException(args.Reason.ToString());
                };
                client.RequestTimedOut += (sender, args) =>
                {
                    Console.WriteLine("client.RequestTimedOut");
                    throw new ApplicationException(args.ToString());
                };
                client.AssociationReleased += (sender, args) =>
                {
                    Console.WriteLine("client.AssociationReleased");
                };

                await client.AddRequestAsync(findRequest);
                await client.SendAsync();

                return result;
            }
            catch (SocketException exception)
            {
                if (exception.Message.Contains("not properly respond"))
                {
                    MessageBoxHelper.ShowError("Error", "Connection to PACS server could not be established");

                    return default;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception during dicom query");
                throw new ApplicationException("Dicom query failed", exception);
            }
        }
    }
}
