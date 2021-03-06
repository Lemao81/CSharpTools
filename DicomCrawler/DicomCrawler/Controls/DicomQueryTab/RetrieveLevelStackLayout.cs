﻿using System;
using DicomCrawler.Enums;
using DicomCrawler.Helpers;
using DicomCrawler.ViewModels;
using Eto.Forms;

namespace DicomCrawler.Controls.DicomQueryTab
{
    public class RetrieveLevelStackLayout : StackLayout
    {
        private RetrieveLevelStackLayout()
        {
        }

        public static RetrieveLevelStackLayout Create()
        {
            var radioButtonList = new EnumRadioButtonList<RetrieveLevel>
            {
                Orientation = Orientation.Vertical,
                GetText = GetRadioButtonText,
                SelectedValue = RetrieveLevel.Patient,
                Spacing = Gap.Vertical.Small
            };
            radioButtonList.BindDataContext(c => c.SelectedValue, (DicomQueryViewModel query) => query.RetrieveLevel);

            return new RetrieveLevelStackLayout
            {
                Spacing = Gap.Medium,
                Items =
                {
                    new StackLayoutItem
                    {
                        Control = new Label
                        {
                            Text = "Retrieve Level:"
                        }
                    },
                    new StackLayoutItem
                    {
                        Control = radioButtonList
                    }
                }
            };
        }

        public static string GetRadioButtonText(RetrieveLevel retrieveLevel) =>
            retrieveLevel switch
            {
                RetrieveLevel.Patient => "Patient",
                RetrieveLevel.Study => "Study",
                RetrieveLevel.Series => "Series",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
