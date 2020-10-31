using System;

namespace DicomCrawler.Models
{
    public class ViewModelEventArgs<T> : EventArgs
    {
        public T ViewModel { get; set; }

        public ViewModelEventArgs(T viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
