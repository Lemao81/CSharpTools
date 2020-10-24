using Eto.Forms;

namespace DicomCrawler
{
    public partial class MainForm
    {
        public MenuBar CreateMenuBar()
        {
            var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
            clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => CreateAboutDialog().ShowDialog(this);

            return new MenuBar
            {
                Items =
                {
                    // File submenu
                    new ButtonMenuItem
                    {
                        Text = "&File", Items = { clickMe }
                    },
                    // new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
                    // new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
                },
                ApplicationItems =
                {
                    // application (OS X) or file menu (others)
                    new ButtonMenuItem
                    {
                        Text = "&Preferences..."
                    },
                },
                QuitItem = quitCommand,
                AboutItem = aboutCommand
            };
        }
    }
}
