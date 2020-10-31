using DicomCrawler.Helpers;
using DicomCrawler.ViewModels;
using Eto.Forms;

namespace DicomCrawler.Controls.DicomQueryTab
{
    public class DicomTagListDynamicLayout : DynamicLayout
    {
        private DicomTagListDynamicLayout()
        {
        }

        public static DicomTagListDynamicLayout Create()
        {
            var dynamicLayout = new DicomTagListDynamicLayout();
            dynamicLayout.BeginVertical();
            dynamicLayout.BeginHorizontal();

            var dicomTagTextBox = CreateDicomTagTextBox();
            var dicomTagList = CreateDicomTagList();
            dynamicLayout.Add(dicomTagTextBox);
            dynamicLayout.Add(CreateAddDicomTagButton(dicomTagTextBox));
            dynamicLayout.Add(CreateRemoveDicomTagButton(dicomTagList));

            dynamicLayout.EndHorizontal();
            dynamicLayout.EndVertical();

            dynamicLayout.BeginVertical();
            dynamicLayout.BeginHorizontal();

            dynamicLayout.Add(dicomTagList);

            dynamicLayout.EndHorizontal();
            dynamicLayout.EndVertical();

            return dynamicLayout;
        }

        private static TextBox CreateDicomTagTextBox()
        {
            var dicomTagTextBox = new TextBox
            {
                PlaceholderText = "Add Tag"
            };
            dicomTagTextBox.TextBinding.BindDataContext((DicomQueryViewModel query) => query.DicomTagInput);

            return dicomTagTextBox;
        }

        private static Button CreateAddDicomTagButton(TextBox dicomTagInput)
        {
            var addDicomTagButton = new Button
            {
                // TODO make it work
                Width = Constants.DicomTagListControlButtonWidth,
                Text = "+"
            };
            addDicomTagButton.Click += (sender, args) =>
            {
                if (dicomTagInput.Text.IsNullOrEmpty())
                {
                    return;
                }

                MvvmHelper.ChangeViewModel<DicomQueryViewModel>(sender, query => query.DicomTags.Add(dicomTagInput.Text), DicomQueryViewModel.OnViewModelChanged);
            };

            return addDicomTagButton;
        }

        private static Button CreateRemoveDicomTagButton(ListBox dicomTagList)
        {
            var removeDicomTagButton = new Button
            {
                // TODO make it work
                Width = Constants.DicomTagListControlButtonWidth,
                Text = "-"
            };
            removeDicomTagButton.Click += (sender, args) =>
            {
                if (dicomTagList.SelectedIndex >= 0 && dicomTagList.SelectedValue is string selectedValue)
                {
                    MvvmHelper.ChangeViewModel<DicomQueryViewModel>(sender, query => query.DicomTags.Remove((selectedValue)), DicomQueryViewModel.OnViewModelChanged);
                }
            };

            return removeDicomTagButton;
        }

        private static ListBox CreateDicomTagList()
        {
            var listBox = new ListBox
            {
            };
            listBox.BindDataContext(c => c.DataStore, (DicomQueryViewModel query) => query.DicomTags);

            return listBox;
        }
    }
}
