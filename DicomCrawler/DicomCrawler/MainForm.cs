using System;
using DicomCrawler.Controls;
using DicomCrawler.Controls.App;
using DicomCrawler.Controls.DicomQueryTab;
using DicomCrawler.Controls.PacsConfigurationTab;
using DicomCrawler.Controls.QueryResultTab;
using DicomCrawler.Helpers;
using DicomCrawler.Models;
using Eto.Forms;
using Eto.Drawing;

namespace DicomCrawler
{
    public class MainForm : Form
    {
        private readonly Version _currentVersion = new Version(0, 0, 1);

        public MainForm()
        {
            InitializeApp();

            Title = "Dicom Crawler";
            Size = new Size(600, 450);
            Location = GetCenterLocation();
            Padding = Gap.Small;
            Content = CreateContent();
            Menu = AppMenuBar.Create(this, _currentVersion);
            // ToolBar = AppToolBar.Create();
        }

        private Point GetCenterLocation() => new Point((int) (Screen.WorkingArea.Width / 2 - Size.Width / 2f), (int) (Screen.WorkingArea.Height / 2 - Size.Height / 2f));

        private static Control CreateContent() =>
            new TabControl
            {
                Pages =
                {
                    DicomQueryTabPage.Create(),
                    DicomQueryResultTabPage.Create(),
                    PacsConfigurationTabPage.Create()
                }
            };

        private static void InitializeApp()
        {
            Application.Instance.Properties.Set(Settings.SettingsKey, new Settings());
            Application.Instance.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, Eto.UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unexpected error occurred", MessageBoxType.Error);
        }
    }
}
