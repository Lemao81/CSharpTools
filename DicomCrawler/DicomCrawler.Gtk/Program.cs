using System;
using Eto.Forms;

namespace DicomCrawler.Gtk
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Gtk).Run(new MainForm());
        }
    }
}
