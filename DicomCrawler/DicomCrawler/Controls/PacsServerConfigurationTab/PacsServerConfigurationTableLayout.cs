using System;
using System.Linq.Expressions;
using DicomCrawler.Models;
using Eto.Drawing;
using Eto.Forms;

namespace DicomCrawler.Controls.PacsServerConfigurationTab
{
    public class PacsServerConfigurationTableLayout : TableLayout
    {
        private PacsServerConfigurationTableLayout() : base(2, 4)
        {
        }

        public static PacsServerConfigurationTableLayout Create()
        {
            var table = new PacsServerConfigurationTableLayout
            {
                Spacing = new Size(8, 4)
            };

            table.Add(CreateLabel("Host:"), 0, 0);
            table.Add(CreateLabel("Port:"), 0, 1);
            table.Add(CreateLabel("CallingAet:"), 0, 2);
            table.Add(CreateLabel("CalledAet:"), 0, 3);

            table.Add(CreateTextBox(_ => _.Host), 1, 0);
            table.Add(CreateTextBox(_ => _.Port), 1, 1);
            table.Add(CreateTextBox(_ => _.CallingAet), 1, 2);
            table.Add(CreateTextBox(_ => _.CalledAet), 1, 3);

            return table;
        }

        private static Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private static TextBox CreateTextBox(Expression<Func<PacsServerConfiguration, string>> propertyExpression)
        {
            var textBox = new TextBox();
            textBox.BindDataContext(_ => _.ReadOnly, (PacsServerConfiguration configuration) => configuration.IsReadOnly);
            textBox.TextBinding.BindDataContext(propertyExpression);

            return textBox;
        }
    }
}
