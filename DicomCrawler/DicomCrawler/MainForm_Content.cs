using Eto.Forms;

namespace DicomCrawler
{
    public partial class MainForm
    {
        public Control CreateContent()
        {
            return new StackLayout
            {
                Padding = 10,
                Items =
                {
                    "Hello World!",
                    // add more controls here
                }
            };
        }
    }
}
