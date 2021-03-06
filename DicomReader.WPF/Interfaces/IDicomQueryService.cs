﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DicomReader.WPF.Models;
using DicomReader.WPF.ViewModels;

namespace DicomReader.WPF.Interfaces
{
    public interface IDicomQueryService
    {
        Task<List<DicomResultSet>> ExecuteDicomQuery(QueryPanelTabUserControlViewModel viewModel,
            PacsConfiguration pacsConfiguration);
    }
}
