using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DockerConductor.Extensions
{
    public static class ViewExtensions
    {
        public static void SwitchToTab(this TabControl? tabControl, int index)
        {
            if (tabControl != null)
            {
                tabControl.SelectedIndex = index;
            }
        }

        public static void Check(this ToggleButton? toggleButton)
        {
            if (toggleButton != null)
            {
                toggleButton.IsChecked = true;
            }
        }

        public static void UnCheck(this ToggleButton? toggleButton)
        {
            if (toggleButton != null)
            {
                toggleButton.IsChecked = false;
            }
        }
    }
}
