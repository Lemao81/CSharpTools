using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dicom;
using Dicom.Network;
using DicomReader.Models;
using DicomClient = Dicom.Network.Client.DicomClient;
using DicomServer = DicomReader.Models.DicomServer;

namespace DicomReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var dicomServer = new DicomServer
            {
                Ip = "192.168.35.60",
                Port = 5678,
                CallingApplicationEntity = "MYSELF",
                CalledApplicationEntity = "CONQUESTSRV1"
            };
            var filePath = @"C:\Dicom\data\HWS0001\1.3.12.2.1107.5.2.18.42051.2018110207163859232900533.0.0.0_0001_000001_158637317400e1.dcm";
            var queryData = new StudyQueryData
            {
                PatiendId = "0009703828",
                StudyInstanceUid = "1.3.46.670589.5.2.10.2156913941.892665384.993397",
                SeriesInstanceUid = "1.3.46.670589.5.2.10.2156913941.892665339.860724"
            };
            var queryData2 = new StudyQueryData
            {
                PatiendId = "KNI0001",
                StudyInstanceUid = "1.3.6.1.4.1.24930.2.64893540834227.1862430"
            };
            var queryData3 = new StudyQueryData
            {
                PatiendId = "HWS0001",
                // StudyInstanceUid = "1.3.6.1.4.1.24930.2.64889795071781.1840298"
            };
            // Task.WaitAll(ExecuteFindPatientRequestAsync(dicomServer, queryData, PrintDataset));
            Task.WaitAll(ExecuteFindStudyRequestAsync(dicomServer, queryData, PrintDataset));
            // Task.WaitAll(ExecuteFindSeriesRequestAsync(dicomServer, queryData, PrintDataset));
            // Task.WaitAll(ExecuteFindWorklistRequestAsync(dicomServer, queryData, PrintDataset));
            // Task.WaitAll(ExecuteFindImageRequestAsync(dicomServer, queryData, PrintDataset));
            // Task.WaitAll(ExecuteFileQuery(filePath, PrintDataset));
        }

        private static async Task ExecuteFindPatientRequestAsync(DicomServer server, StudyQueryData queryData, Action<DicomDataset> datasetAction)
        {
            var findRequest = DicomCFindRequest.CreatePatientQuery(queryData.PatiendId);
            findRequest.OnResponseReceived = (request, response) => datasetAction(response.Dataset);
            var client = new DicomClient(server.Ip, server.Port, false, server.CallingApplicationEntity, server.CalledApplicationEntity);
            await client.AddRequestAsync(findRequest);
            await client.SendAsync();
        }

        private static async Task ExecuteFindStudyRequestAsync(DicomServer server, StudyQueryData queryData, Action<DicomDataset> datasetAction)
        {
            var findRequest = DicomCFindRequest.CreateStudyQuery(queryData.PatiendId, studyInstanceUid: queryData.StudyInstanceUid);
            findRequest.OnResponseReceived = (request, response) => datasetAction(response.Dataset);
            var client = new DicomClient(server.Ip, server.Port, false, server.CallingApplicationEntity, server.CalledApplicationEntity);
            await client.AddRequestAsync(findRequest);
            await client.SendAsync();
        }

        private static async Task ExecuteFindSeriesRequestAsync(DicomServer server, StudyQueryData queryData, Action<DicomDataset> datasetAction)
        {
            var findRequest = DicomCFindRequest.CreateSeriesQuery(queryData.StudyInstanceUid);
            findRequest.OnResponseReceived = (request, response) => datasetAction(response.Dataset);
            var client = new DicomClient(server.Ip, server.Port, false, server.CallingApplicationEntity, server.CalledApplicationEntity);
            await client.AddRequestAsync(findRequest);
            await client.SendAsync();
        }

        private static async Task ExecuteFindWorklistRequestAsync(DicomServer server, StudyQueryData queryData, Action<DicomDataset> datasetAction)
        {
            var findRequest = DicomCFindRequest.CreateWorklistQuery(queryData.PatiendId);
            findRequest.OnResponseReceived = (request, response) => datasetAction(response.Dataset);
            var client = new DicomClient(server.Ip, server.Port, false, server.CallingApplicationEntity, server.CalledApplicationEntity);
            await client.AddRequestAsync(findRequest);
            await client.SendAsync();
        }

        private static async Task ExecuteFindImageRequestAsync(DicomServer server, StudyQueryData queryData, Action<DicomDataset> datasetAction)
        {
            var findRequest = DicomCFindRequest.CreateImageQuery(queryData.StudyInstanceUid, queryData.SeriesInstanceUid);
            findRequest.OnResponseReceived = (request, response) => datasetAction(response.Dataset);
            var client = new DicomClient(server.Ip, server.Port, false, server.CallingApplicationEntity, server.CalledApplicationEntity);
            await client.AddRequestAsync(findRequest);
            await client.SendAsync();
        }

        private static void PrintDataset(DicomDataset dataset)
        {
            using var enumerator = dataset.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var tag = enumerator.Current!.Tag;
                var name = tag.DictionaryEntry.Name;
                try
                {
                    var singleValue = dataset.GetSingleValue<dynamic>(tag);
                    Console.WriteLine($"{name}: {singleValue}");
                }
                catch (Exception)
                {
                    try
                    {
                        var values = dataset.GetValues<dynamic>(tag);
                        Console.WriteLine($"{name}: {string.Join(", ", values.Take(5).Select(_ => _.ToString()))}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(tag.DictionaryEntry.Name + " not available");
                    }
                }
            }
        }

        private static async Task ExecuteFileQuery(string filePath, Action<DicomDataset> datasetAction) =>
            await DicomFile.OpenAsync(filePath).ContinueWith(task => datasetAction(task.Result.Dataset));

        private static List<DicomTag> GetDicomTags()
        {
            return typeof(DicomTag).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(_ => _.FieldType == typeof(DicomTag))
                .Select(_ => _.GetValue(null))
                .Cast<DicomTag>()
                .ToList();
        }

        private static List<DicomUID> GetDicomUids() => DicomUID.Enumerate().ToList();

        private static IEnumerable<(string, T)> GetSingleValues<T>(DicomDataset dataset, IEnumerable<DicomTag> dicomTags)
        {
            var result = new List<(string, T)>();
            foreach (var dicomTag in dicomTags)
            {
                try
                {
                    result.Add((dicomTag.DictionaryEntry.Name, dataset.GetSingleValue<T>(dicomTag)));
                }
                catch (Exception)
                {
                }
            }

            return result;
        }
    }
}