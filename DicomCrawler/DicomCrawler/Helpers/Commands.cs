using Eto.Forms;

namespace DicomCrawler.Helpers
{
    public static class Commands
    {
        public static Command ClickMe(Control parent)
        {
            var command = new Command
            {
                MenuText = "Click Me!",
                ToolBarText = "Click Me!"
            };
            command.Executed += (sender, e) => MessageBox.Show(parent, "I was clicked!");

            return command;
        }

        public static Command QuitApp(Control parent)
        {
            var command = new Command
            {
                MenuText = "Quit",
                Shortcut = Application.Instance.CommonModifier | Keys.Q
            };
            command.Executed += (sender, e) => Application.Instance.Quit();

            return command;
        }

        public static Command ShowAboutDialog(Control parent, AboutDialog aboutDialog)
        {
            var command = new Command
            {
                MenuText = "About..."
            };
            command.Executed += (sender, e) => aboutDialog.ShowDialog(parent);

            return command;
        }
    }
}
