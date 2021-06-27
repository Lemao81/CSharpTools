using System;
using Avalonia;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class DicomRequestFactoryProvider : IDicomRequestFactoryProvider
    {
        public IDicomRequestFactory ProvideFactory(DicomQueryInputs inputs)
        {
            switch (inputs.DicomRequestType)
            {
                case DicomRequestType.None:
                    throw new InvalidOperationException("Request type not selected");
                case DicomRequestType.CFind:
                    return AvaloniaLocator.CurrentMutable.GetService<IDicomCFindRequestFactoryProvider>().ProvideFactory(inputs);
                case DicomRequestType.CGet:
                    throw new NotImplementedException();
                case DicomRequestType.CMove:
                    return AvaloniaLocator.CurrentMutable.GetService<IDicomCMoveRequestFactory>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
