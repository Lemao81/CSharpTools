using System.Collections.ObjectModel;
using DicomReader.WPF.Models;
using Prism.Mvvm;

namespace DicomReader.WPF.ViewModels
{
    public class ListResultTabUserControlViewModel : BindableBase
    {
        public ListResultTabUserControlViewModel()
        {
            Results = new ObservableCollection<DicomResultSet>();
            QueryPanelTabUserControlViewModel.ResultChanged += (sender, args) =>
            {
                Results.Clear();
                Results.AddRange(args.Results);
            };
        }

        #region bound properties
        public ObservableCollection<DicomResultSet> Results { get; }
        #endregion
    }
}
