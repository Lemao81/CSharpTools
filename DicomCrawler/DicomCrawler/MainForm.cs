using Eto.Forms;
using Eto.Drawing;

namespace DicomCrawler
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            Title = "Dicom Crawler";
            ClientSize = new Size(400, 350);

            Content = CreateContent();
            Menu = CreateMenuBar();
            ToolBar = CreateToolBar();
        }
    }
}
