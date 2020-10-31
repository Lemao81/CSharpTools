using System;
using System.IO;
using DicomCrawler.Helpers;
using DicomCrawler.Models;
using DicomCrawler.ViewModels;
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
            var tabPage = new PacsConfigurationTabPage
            {
                Padding = Gap.Medium,
                DataContext = ViewModelFactory.CreateEmpty<PacsConfigurationViewModel>(),
                Content = new StackLayout
                {
                    Items =
                    {
                        new StackLayoutItem { Control = PacsConfigurationTableLayout.Create() },
                        new StackLayoutItem
                        {
                            Control = new StackLayout
                            {
                                Padding = new Padding(0, 8, 0, 0),
                                Orientation = Orientation.Horizontal,
                                Items =
                                {
                                    new StackLayoutItem { Control = CreateModifyButton() },
                                    new StackLayoutItem { Control = CreateSaveButton() }
                                }
                            }
                        }
                    }
                }
            };

            PacsConfigurationViewModel.ViewModelChanged += (sender, args) => tabPage.DataContext = args.ViewModel;

            return tabPage;
        }

        private static Button CreateModifyButton()
        {
            var modifyButton = new Button
            {
                Text = "Modify"
            };
            modifyButton.Click += (sender, args) => MvvmHelper.ChangeViewModel<PacsConfigurationViewModel>(sender, configuraiton => configuraiton.IsReadOnly = false,
                PacsConfigurationViewModel.OnViewModelChanged);
            modifyButton.BindDataContext(b => b.Enabled, (PacsConfigurationViewModel configuration) => configuration.IsReadOnly);

            return modifyButton;
        }

        private static Button CreateSaveButton()
        {
            var saveButton = new Button
            {
                Text = "Save",
            };
            saveButton.Click += (sender, args) => OnSave(sender);
            saveButton.BindDataContext(b => b.Enabled, (PacsConfigurationViewModel configuration) => !configuration.IsReadOnly);

            return saveButton;
        }

        private static void OnModify(PacsConfigurationViewModel configuration)
        {
            configuration.IsReadOnly = false;
        }

        private static void OnSave(object sender)
        {
            var configuration = MvvmHelper.GetDataContextViewModel<PacsConfigurationViewModel>(sender);
            var settings = Application.Instance.GetSettings();
            try
            {
                SetSettings(configuration, settings);
                WriteSettingsToFile(settings);
                MvvmHelper.ChangeViewModel<PacsConfigurationViewModel>(sender, configuraiton => configuraiton.IsReadOnly = true,
                    PacsConfigurationViewModel.OnViewModelChanged);
            }
            catch (Exception)
            {
                MessageBox.Show("Input is not valid", MessageBoxType.Error);
            }
        }

        private static void SetSettings(PacsConfigurationViewModel configuration, Settings settings)
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
        }

        private static void WriteSettingsToFile(Settings settings)
        {
            var settingsIO = new SettingsIO(settings);
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settingsIO, Formatting.Indented));
        }
    }
}
