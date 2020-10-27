using DicomCrawler.Helpers;
using DicomCrawler.Models;
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
            return new DicomQueryTabPage
            {
                Padding = Gap.Medium,
                DataContext = ViewModelFactory.Create<DicomQuery>(),
                Content = new StackLayout
                {
                    Orientation = Orientation.Vertical,
                    Spacing = Gap.Large,
                    Width = 300,
                    Items =
                    {
                        new StackLayoutItem
                        {
                            Control = new StackLayout
                            {
                                Orientation = Orientation.Horizontal,
                                Items =
                                {
                                    new StackLayoutItem
                                    {
                                        Control = RetrieveLevelStackLayout.Create()
                                    }
                                }
                            }
                        },
                        new StackLayoutItem
                        {
                            Control = new StackLayout
                            {
                                Orientation = Orientation.Horizontal,
                                Items =
                                {
                                    new StackLayoutItem
                                    {
                                        Control = QueryParameterStackLayout.Create()
                                    }
                                }
                            }
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
            };
        }
    }
}
