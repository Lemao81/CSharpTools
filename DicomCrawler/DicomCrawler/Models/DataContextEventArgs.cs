using System;

namespace DicomCrawler.Models
{
    public class DataContextEventArgs<T> : EventArgs
    {
        public T DataContext { get; set; }

        public DataContextEventArgs(T dataContext)
        {
            DataContext = dataContext;
        }
    }
}
