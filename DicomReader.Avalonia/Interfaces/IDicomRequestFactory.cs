﻿using System;
using System.Threading;
using Dicom.Network;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomRequestFactory
    {
        DicomRequest CreateRequest(
            DicomQueryInputs inputs,
            PacsConfiguration pacsConfiguration,
            IDicomResponseCollector responseCollector,
            CancellationTokenSource cts,
            Action<DicomRequest, DicomResponse, IDicomResponseCollector, CancellationTokenSource> responseAction
        );
    }
}
