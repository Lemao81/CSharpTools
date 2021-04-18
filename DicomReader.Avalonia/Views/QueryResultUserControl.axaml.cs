using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DicomReader.Avalonia.Views
{
    public class QueryResultUserControl : UserControl
    {
        public QueryResultUserControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}