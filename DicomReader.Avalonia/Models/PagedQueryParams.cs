using System;

namespace DicomReader.Avalonia.Models
{
    public class PagedQueryParams
    {
        public PagedQueryParams(bool isPaged, int? pageSize)
        {
            if (isPaged && !pageSize.HasValue) throw new InvalidOperationException("Page size needed if query is paged");

            IsPaged = isPaged;
            PageSize = pageSize;
        }

        public bool IsPaged { get; }
        public int? PageSize { get; }
    }
}
