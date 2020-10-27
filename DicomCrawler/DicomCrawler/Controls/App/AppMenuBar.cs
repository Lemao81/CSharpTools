using System;
using DicomCrawler.Helpers;
using Eto.Forms;

namespace DicomCrawler.Controls.App
{
    public class AppMenuBar : MenuBar
    {
        private AppMenuBar()
        {
        }

        public static AppMenuBar Create(Control parent, Version currentVersion)
        {
            return new AppMenuBar
            {
                Items =
                {
                    new ButtonMenuItem
                    {
                        Text = "&File", Items = { Commands.ClickMe(parent) }
                    },
                },
                ApplicationItems =
                {
                    new ButtonMenuItem
                    {
                        Text = "&Preferences..."
                    },
                },
                QuitItem = Commands.QuitApp(parent),
                AboutItem = Commands.ShowAboutDialog(parent, CreateAboutDialog(currentVersion))
            };
        }

        private static AboutDialog CreateAboutDialog(Version _currentVersion) =>
            new AboutDialog
            {
                Developers = new[] { "Jürgen Reiser" },
                Title = "Dicom Crawler",
                ProgramName = "Dicom Crawler",
                Version = $"Version: {_currentVersion}"
            };
    }
}
