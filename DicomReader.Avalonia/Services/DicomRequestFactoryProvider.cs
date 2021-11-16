using System;
using Avalonia;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class DicomRequestFactoryProvider : IDicomRequestFactoryProvider
    {
        private readonly IDicomCFindRequestFactoryProvider _dicomCFindRequestFactoryProvider;
        private readonly IDicomCMoveRequestFactory         _dicomCMoveRequestFactory;

        public DicomRequestFactoryProvider()
        {
            _dicomCFindRequestFactoryProvider = AvaloniaLocator.CurrentMutable.GetService<IDicomCFindRequestFactoryProvider>();
            _dicomCMoveRequestFactory         = AvaloniaLocator.CurrentMutable.GetService<IDicomCMoveRequestFactory>();
        }

        public IDicomRequestFactory ProvideFactory(DicomQueryInputs inputs)
        {
            switch (inputs.DicomRequestType)
            {
                case DicomRequestType.None:
                    throw new InvalidOperationException("Request type not selected");
                case DicomRequestType.CFind:
                    return _dicomCFindRequestFactoryProvider.ProvideFactory(inputs);
                case DicomRequestType.CGet:
                    throw new NotImplementedException();
                case DicomRequestType.CMove:
                    return _dicomCMoveRequestFactory;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
