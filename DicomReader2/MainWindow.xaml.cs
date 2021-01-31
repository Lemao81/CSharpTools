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
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _mainViewModel.Init();
        }
    }
}
