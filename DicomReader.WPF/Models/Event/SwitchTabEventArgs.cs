using System;

namespace DicomReader.WPF.Models.Event
{
    public class SwitchTabEventArgs : EventArgs
    {
        public SwitchTabEventArgs(int index)
        {
            SelectedIndex = index;
        }

        public int SelectedIndex { get; set; }
    }
}
