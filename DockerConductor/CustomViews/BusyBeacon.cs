using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DockerConductor.CustomViews
{
    public class BusyBeacon : Border
    {
        public BusyBeacon()
        {
            Child = new TextBlock
            {
                Height     = 35,
                Width      = 35,
                Background = Brushes.Green
            };

            BorderBrush     = Brushes.DarkGray;
            BorderThickness = new Thickness(2);
        }

        public void SwitchBusy()
        {
            if (Child is TextBlock child)
            {
                child.Background = Brushes.Red;
            }
        }

        public void SwitchIdle()
        {
            if (Child is TextBlock child)
            {
                child.Background = Brushes.Green;
            }
        }
    }
}
