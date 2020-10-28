using DicomCrawler.Helpers;
using DicomCrawler.ViewModels;
using Eto.Forms;

namespace DicomCrawler.Controls.DicomQueryTab
{
    public class QueryParameterStackLayout : StackLayout
    {
        private QueryParameterStackLayout()
        {
        }

        public static QueryParameterStackLayout Create()
        {
            var patientIdTextBox = new TextBox();
            patientIdTextBox.TextBinding.BindDataContext((DicomQueryViewModel query) => query.Parameter.PatientId);

            var accessionNumberTextBox = new TextBox();
            accessionNumberTextBox.TextBinding.BindDataContext((DicomQueryViewModel query) => query.Parameter.AccessionNumber);

            var studyInstanceUidTextBox = new TextBox();
            studyInstanceUidTextBox.TextBinding.BindDataContext((DicomQueryViewModel query) => query.Parameter.StudyInstanceUid);

            return new QueryParameterStackLayout
            {
                Spacing = Gap.Medium,
                Items =
                {
                    new StackLayoutItem
                    {
                        Control = new Label
                        {
                            Text = "Parameter:"
                        }
                    },
                    new StackLayoutItem
                    {
                        Control = new StackLayout
                        {
                            Spacing = Gap.Small,
                            Items =
                            {
                                new StackLayoutItem
                                {
                                    Control = new StackLayout
                                    {
                                        Spacing = Gap.Small,
                                        Items =
                                        {
                                            new StackLayoutItem
                                            {
                                                Control = new Label
                                                {
                                                    Text = "Patient Id:"
                                                }
                                            },
                                            new StackLayoutItem
                                            {
                                                Control = patientIdTextBox
                                            }
                                        }
                                    }
                                },
                                new StackLayoutItem
                                {
                                    Control = new StackLayout
                                    {
                                        Spacing = Gap.Small,
                                        Items =
                                        {
                                            new StackLayoutItem
                                            {
                                                Control = new Label
                                                {
                                                    Text = "Accession Number:"
                                                }
                                            },
                                            new StackLayoutItem
                                            {
                                                Control = accessionNumberTextBox
                                            }
                                        }
                                    }
                                },
                                new StackLayoutItem
                                {
                                    Control = new StackLayout
                                    {
                                        Spacing = Gap.Small,
                                        Items =
                                        {
                                            new StackLayoutItem
                                            {
                                                Control = new Label
                                                {
                                                    Text = "Study Instance Uid:"
                                                }
                                            },
                                            new StackLayoutItem
                                            {
                                                Control = studyInstanceUidTextBox
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
