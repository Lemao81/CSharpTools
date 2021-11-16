using System;
using Dicom.Network.Client.EventArguments;
using DicomReader.Avalonia.Constants;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DicomReader.Avalonia.Factories
{
    public class DicomClientFactory : IDicomClientFactory
    {
        public DicomClient CreateClient(DicomRequestType requestType, PacsConfiguration pacsConfig)
        {
            var callingAe = string.IsNullOrEmpty(pacsConfig.CallingAe) ? GetCallingAeDefault(requestType) : pacsConfig.CallingAe;
            var client    = new DicomClient(pacsConfig.Host, pacsConfig.Port, false, callingAe, pacsConfig.CalledAe);
            client.NegotiateAsyncOps();

            client.StateChanged        += (_, args) => OnStateChanged(args);
            client.AssociationAccepted += (_, args) => OnAssociationAccepted(args);
            client.AssociationRejected += (_, args) => OnAssociationRejected(args);
            client.AssociationReleased += (_, _) => OnAssociationReleased();
            client.RequestTimedOut     += (_, args) => OnRequestTimedOut(args);

            return client;
        }

        private static string GetCallingAeDefault(DicomRequestType requestType) => requestType switch
        {
            DicomRequestType.None  => throw new InvalidOperationException("No request type specified"),
            DicomRequestType.CFind => Consts.FindScu,
            DicomRequestType.CGet  => Consts.GetScu,
            DicomRequestType.CMove => Consts.MoveScu,
            _                      => throw new ArgumentOutOfRangeException($"Invalid query request {requestType}")
        };

        private static void OnStateChanged(StateChangedEventArgs args) => AuditTrailEntry.Emit(args.NewState?.ToString());

        private static void OnAssociationAccepted(AssociationAcceptedEventArgs args) => AuditTrailEntry.Emit(
            $"ASSOCIATION ACCEPTED. Host: {args.Association.RemoteHost} "
            + $"Port: {args.Association.RemotePort} CalledAE: {args.Association.CalledAE} "
            + $"CallingAE: {args.Association.CallingAE}"
        );

        private static void OnAssociationRejected(AssociationRejectedEventArgs args)
        {
            AuditTrailEntry.Emit($"ASSOCIATION REJECTED. Reason: {args.Reason}");
            throw new ApplicationException(args.Reason.ToString());
        }

        private static void OnRequestTimedOut(RequestTimedOutEventArgs args)
        {
            AuditTrailEntry.Emit("CLIENT TIMED OUT");
            throw new ApplicationException(args.ToString());
        }

        private static void OnAssociationReleased() => AuditTrailEntry.Emit("ASSOCIATION RELEASED");
    }
}
