using System;
using System.Windows.Controls;
using DicomReader.WPF.ViewModels;

namespace DicomReader.WPF.Views
{
    public partial class QueryPanelTabUserControl
    {
        public QueryPanelTabUserControl()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (QueryPanelTabUserControlViewModel) DataContext;
            foreach (string addedItem in e.AddedItems)
            {
                viewModel.SelectedRequestedFields.Add(addedItem);
            }

            viewModel.SelectedRequestedFields.RemoveAll(f => e.RemovedItems.Contains(f));
        }
    }
}
