using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Avalonia;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public class DicomRequestFactory : IDicomRequestFactory
    {
        public DicomRequest CreateRequest(
            DicomQueryInputs inputs,
            PacsConfiguration pacsConfiguration,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cts,
            Action<DicomRequest, DicomResponse, IDicomResponseCollector, CancellationTokenSource> responseAction
        )
        {
            var request = AvaloniaLocator.CurrentMutable.GetService<IDicomRequestFactoryProvider>()
                .ProvideFactory(inputs)
                .CreateRequest(inputs, pacsConfiguration, responseCollector, cts, responseAction);

            request.OnTimeout = (_, _) => OnTimeout();
            request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");

            AddRequestedDicomTags(inputs.DicomQueryParams.RequestedDicomTags, request);

            return request;
        }

        private static void OnTimeout() => AuditTrailEntry.Emit("REQUEST TIMED OUT");

        private static void AddRequestedDicomTags(IEnumerable<DicomTagItem> requestedDicomTags, DicomMessage request)
        {
            foreach (var tagContent in requestedDicomTags.Select(t => t.Content.Trim()))
            {
                AvaloniaLocator.Current.GetService<IDicomTagProvider>().ProvideDicomTag(tagContent)
                    .OnSuccess(dicomTag => AddDicomTagToRequestIfNotExist(request, dicomTag))
                    .OnException(exception => throw exception);
            }
        }

        private static void AddDicomTagToRequestIfNotExist(DicomMessage findRequest, DicomTag tag)
        {
            if (findRequest.Dataset.Contains(tag)) return;

            var accordingEntry = DicomDictionary.Default.FirstOrDefault(_ => _.Tag == tag);

            var vr = accordingEntry?.ValueRepresentations.First();
            if (vr?.ValueType == null) return;

            var addMethod = typeof(DicomRequestFactory).GetMethod(nameof(DatasetAddGeneric), BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(vr.ValueType);
            addMethod?.Invoke(null, new object[] { findRequest, tag });
        }

        private static void DatasetAddGeneric<T>(DicomMessage findRequest, DicomTag tag) => findRequest.Dataset.AddOrUpdate<T>(tag);
    }
}
