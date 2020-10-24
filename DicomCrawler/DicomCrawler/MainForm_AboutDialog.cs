using System;
using Eto.Forms;

namespace DicomCrawler
{
    public partial class MainForm
    {
        private readonly Version _currentVersion = new Version(0, 0, 1);

        public AboutDialog CreateAboutDialog() =>
            new AboutDialog
            {
                Developers = new[] { "Jürgen Reiser" };
                Title = "Dicom Crawler";
                ProgramName = "Dicom Crawler";
                Version = $"Version: {_currentVersion}";
            };
    }
}
