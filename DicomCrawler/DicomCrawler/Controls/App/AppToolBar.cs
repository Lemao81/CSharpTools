using Eto.Forms;

namespace DicomCrawler.Controls.App
{
    public class AppToolBar : ToolBar
    {
        private AppToolBar()
        {
        }

        public static AppToolBar Create()
        {
            return new AppToolBar
            {
                Items = { }
            };
        }
    }
}
