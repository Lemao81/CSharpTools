using System;
using Dicom.Network;
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
            _mainViewModel.RequestedFieldFocusRequested += OnRequestedFieldFocusRequested;
            _mainViewModel.QueryButtonFocusRequested += OnQueryButtonFocusRequested;
            _mainViewModel.PatientId = "17102";
            _mainViewModel.RetrieveLevel = DicomQueryRetrieveLevel.Study;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _mainViewModel.Init();
        }

        private void OnRequestedFieldFocusRequested(object sender, EventArgs args) => txtRequestedField.Focus();

        private void OnQueryButtonFocusRequested(object sender, EventArgs args) => btnQuery.Focus();
    }
}
