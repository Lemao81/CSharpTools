using Eto.Forms;

namespace DicomCrawler.Controls.QueryResultTab
{
    public class DicomQueryResultTabPage : TabPage
    {
        private DicomQueryResultTabPage()
        {
            Text = "Result";
        }

        public static DicomQueryResultTabPage Create()
        {
            return new DicomQueryResultTabPage();
        }
    }
}
