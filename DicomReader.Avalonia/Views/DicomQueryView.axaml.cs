using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DicomReader.Avalonia.ViewModels;
using ReactiveUI;

namespace DicomReader.Avalonia.Views
{
    public class DicomQueryView : UserControl
    {
        public DicomQueryView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SelectingItemsControl_OnSelectionChanged(object? _, SelectionChangedEventArgs __)
        {
            var viewModel = DataContext as DicomQueryViewModel;
            viewModel?.RaisePropertyChanged(nameof(DicomQueryViewModel.CanRemoveRequestedDicomTags));
        }
    }
}
