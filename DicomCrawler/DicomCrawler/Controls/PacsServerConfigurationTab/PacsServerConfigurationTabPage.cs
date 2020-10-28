using System;
using System.IO;
using DicomCrawler.Helpers;
using DicomCrawler.Models;
using Eto.Drawing;
using Eto.Forms;
using Newtonsoft.Json;

namespace DicomCrawler.Controls.PacsServerConfigurationTab
{
    public class PacsServerConfigurationTabPage : TabPage
    {
        private PacsServerConfigurationTabPage()
        {
            Text = "PACS Configuration";
        }

        public static PacsServerConfigurationTabPage Create()
        {
            return new PacsServerConfigurationTabPage
            {
                Padding = Gap.Medium,
                DataContext = ViewModelFactory.Create<PacsServerConfiguration>(),
                Content = new StackLayout
                {
                    Items =
                    {
                        new StackLayoutItem
                        {
                            Control = PacsServerConfigurationTableLayout.Create()
                        },
                        new StackLayoutItem
                        {
                            Control = new StackLayout
                            {
                                Padding = new Padding(0, 8, 0, 0),
                                Orientation = Orientation.Horizontal,
                                Items =
                                {
                                    new StackLayoutItem
                                    {
                                        Control = CreateModifyButton()
                                    },
                                    new StackLayoutItem
                                    {
                                        Control = CreateSaveButton()
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static Button CreateModifyButton()
        {
            var modifyButton = new Button
            {
                Text = "Modify"
            };
            modifyButton.Click += (sender, args) => OnModify((PacsServerConfiguration) ((Button) sender).DataContext);
            modifyButton.BindDataContext(b => b.Enabled, (PacsServerConfiguration configuration) => configuration.IsReadOnly);

            return modifyButton;
        }

        private static Button CreateSaveButton()
        {
            var saveButton = new Button
            {
                Text = "Save",
            };
            saveButton.Click += (sender, args) => OnSave((PacsServerConfiguration) ((Button) sender).DataContext);
            saveButton.BindDataContext(b => b.Enabled, (PacsServerConfiguration configuration) => !configuration.IsReadOnly);

            return saveButton;
        }

        private static void OnModify(PacsServerConfiguration configuration)
        {
            configuration.IsReadOnly = false;
        }

        private static void OnSave(PacsServerConfiguration configuration)
        {
            var settings = Application.Instance.GetSettings();

            try
            {
                var host = configuration.Host;
                var port = configuration.Port.IsNullOrEmpty() ? 0 : int.Parse(configuration.Port);
                var callingAet = configuration.CallingAet;
                var calledAet = configuration.CalledAet;

                settings.PacsHost = host;
                settings.PacsPort = port;
                settings.PacsCallingAet = callingAet;
                settings.PacsCalledAet = calledAet;
                configuration.IsReadOnly = true;

                File.WriteAllText("config.json", JsonConvert.SerializeObject(settings));
            }
            catch (Exception)
            {
                MessageBox.Show("Input is not valid", MessageBoxType.Error);
            }
        }
    }
}
