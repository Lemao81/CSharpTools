using DicomReader2.ViewModels;

namespace DicomReader.WPF
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            var mainViewModel = new MainViewModel();
            DataContext = mainViewModel;
            InitializeComponent();
        }
    }
}
