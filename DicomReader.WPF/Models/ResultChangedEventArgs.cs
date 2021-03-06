﻿using System;
using System.Collections.Generic;

namespace DicomReader.WPF.Models
{
    public class ResultChangedEventArgs : EventArgs
    {
        public ResultChangedEventArgs(List<DicomResultSet> results)
        {
            Results = results;
        }

        public List<DicomResultSet> Results { get; set; }
    }
}
