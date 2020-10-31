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

        public static void ChangeViewModel<T>(object sender, Action<T> changeAction, Action<T> emitOnChanged) where T : class
        {
            var currentViewModel = GetDataContextViewModel<T>(sender);
            var newViewModel = ViewModelFactory.CreateCloned<T>(currentViewModel);
            changeAction(newViewModel);
            emitOnChanged(newViewModel);
        }
    }
}
