using System;

namespace DicomReader.Avalonia.Models
{
    public class PagedQueryParams
    {
        public PagedQueryParams(bool isPaged, int pageSize, int page)
        {
            if (isPaged && (pageSize < 0 || page < 1)) throw new InvalidOperationException("Page size and page needed if query is paged");

            IsPaged = isPaged;
            PageSize = pageSize;
            Page = page;
        }

        public bool IsPaged { get; }
        public int? PageSize { get; }
        public int Page { get; set; }
    }
}
