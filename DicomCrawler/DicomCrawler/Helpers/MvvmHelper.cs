using System;
using Eto.Forms;

namespace DicomCrawler.Helpers
{
    public static class MvvmHelper
    {
        public static T GetDataContextViewModel<T>(object control) where T : class
        {
            if (!(((BindableWidget) control).DataContext is T viewModel))
            {
                throw new ApplicationException("no viewmodel found in datacontext");
            }

            return viewModel;
        }
    }
}
