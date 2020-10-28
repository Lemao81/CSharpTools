using System;
using System.IO;
using DicomCrawler.Helpers;
using DicomCrawler.Models;
using Eto.Drawing;
using Eto.Forms;
using Newtonsoft.Json;

namespace DicomCrawler.Controls.PacsConfigurationTab
{
    public class PacsConfigurationTabPage : TabPage
    {
        private PacsConfigurationTabPage()
        {
            Text = "PACS Configuration";
        }

        public static PacsConfigurationTabPage Create()
        {
            return new PacsConfigurationTabPage
            {
                Padding = Gap.Medium,
                DataContext = ViewModelFactory.Create<PacsConfigurationViewModel>(),
                Content = new StackLayout
                {
                    Items =
                    {
                        new StackLayoutItem
                        {
                            Control = PacsConfigurationTableLayout.Create()
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
            modifyButton.Click += (sender, args) => OnModify((PacsConfigurationViewModel) ((Button) sender).DataContext);
            modifyButton.BindDataContext(b => b.Enabled, (PacsConfigurationViewModel configuration) => configuration.IsReadOnly);

            return modifyButton;
        }

        private static Button CreateSaveButton()
        {
            var saveButton = new Button
            {
                Text = "Save",
            };
            saveButton.Click += (sender, args) => OnSave((PacsConfigurationViewModel) ((Button) sender).DataContext);
            saveButton.BindDataContext(b => b.Enabled, (PacsConfigurationViewModel configuration) => !configuration.IsReadOnly);

            return saveButton;
        }

        private static void OnModify(PacsConfigurationViewModel configuration)
        {
            configuration.IsReadOnly = false;
        }

        private static void OnSave(PacsConfigurationViewModel configuration)
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

                File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented, new JsonSerializerSettings?()));
            }
            catch (Exception)
            {
                MessageBox.Show("Input is not valid", MessageBoxType.Error);
            }
        }
    }
}
