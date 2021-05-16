using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Extensions;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Factories
{
    public abstract class CustomDicomRequestFactory : IDicomCFindRequestFactory
    {
        public DicomCFindRequest CreateCFindRequest(DicomQueryParams queryParams)
        {
            var request = CreateRequestInternal(queryParams);

            // check if necessary
            request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");

            AddRequestedDicomTags(queryParams.RequestedDicomTags, request);

            return request;
        }

        protected abstract DicomCFindRequest CreateRequestInternal(DicomQueryParams queryParams);

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

            var addMethod = typeof(CustomDicomRequestFactory).GetMethod(nameof(DatasetAddGeneric), BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(vr.ValueType);
            addMethod?.Invoke(null, new object[] { findRequest, tag });
        }


        private static void DatasetAddGeneric<T>(DicomMessage findRequest, DicomTag tag) => findRequest.Dataset.AddOrUpdate<T>(tag);
    }
}
