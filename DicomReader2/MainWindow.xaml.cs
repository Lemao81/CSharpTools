using System;
using DicomReader2.ViewModels;

namespace DicomReader2
{
    public partial class MainWindow
    {
        private readonly MainViewModel _mainViewModel = new MainViewModel();

        public MainWindow()
        {
            DataContext = _mainViewModel;
            InitializeComponent();
            _mainViewModel.RequestedFieldFocusRequested += (s, e) => OnRequestedFieldFocusRequested();
            _mainViewModel.PatientId = "17102";
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _mainViewModel.Init();
        }

        private void OnRequestedFieldFocusRequested() => txtRequestedField.Focus();
    }
}
