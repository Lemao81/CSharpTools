using DicomCrawler.Helpers;
using DicomCrawler.ViewModels;
using Eto.Forms;

namespace DicomCrawler.Controls.DicomQueryTab
{
    public class DicomQueryTabPage : TabPage
    {
        private DicomQueryTabPage()
        {
            Text = "Query";
        }

        public static DicomQueryTabPage Create()
        {
            var tabPage = new DicomQueryTabPage
            {
                Padding = Gap.Medium,
                DataContext = ViewModelFactory.CreateEmpty<DicomQueryViewModel>(),
            };

            DicomQueryViewModel.ViewModelChanged += (sender, args) => tabPage.DataContext = args.ViewModel;

            tabPage.Content = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = Gap.Large,
                Items =
                {
                    new StackLayoutItem
                    {
                        Control = new StackLayout
                        {
                            Spacing = Gap.Large,
                            Items =
                            {
                                new StackLayoutItem
                                {
                                    Control = RetrieveLevelStackLayout.Create()
                                },
                                new StackLayoutItem
                                {
                                    Control = QueryParameterStackLayout.Create()
                                },
                                new StackLayoutItem
                                {
                                    Control = new Button
                                    {
                                        Text = "Start Query"
                                    }
                                }
                            }
                        }
                    },
                    new StackLayoutItem
                    {
                        Control = new StackLayout
                        {
                            Items =
                            {
                                new StackLayoutItem
                                {
                                    Control = DicomTagListDynamicLayout.Create()
                                }
                            }
                        }
                    }
                }
            };

            return tabPage;
        }
    }
}
