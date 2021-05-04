using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Common.Extensions;
using Dicom;
using Dicom.Network;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Models;
using DynamicData;
using ReactiveUI;

namespace DicomReader.Avalonia.ViewModels
{
    public class DicomQueryViewModel : ViewModelBase
    {
        private string _requestedDicomTagInput = string.Empty;
        private string _patientId = string.Empty;
        private string _accessionNumber = string.Empty;
        private string _studyInstanceUid = string.Empty;
        private string _pageSize = string.Empty;
        private DicomRequestType _requestType;
        private DicomQueryRetrieveLevel _retrieveLevel = DicomQueryRetrieveLevel.NotApplicable;

        public DicomQueryViewModel()
        {
            ConfigureAddRequestedDicomTagButton();
            ConfigureRemoveRequestedDicomTagsButton();
            ConfigureClearRequestedDicomTagsButton();
            ConfigureArrangeStandardPatientQueryButton();
            ConfigureArrangeStandardStudyQueryButton();
            ConfigureArrangeStandardSeriesQueryButton();
            ConfigureArrangeCustomQueryButton();
            ConfigureStartQueryButton();
            ConfigureStartPagedQueryButton();
            ConfigureTestButton();

            AddRequestedDicomTag?.Subscribe(dicomTag => RequestedDicomTags.Add(dicomTag));
            AuditTrailEntry.Stream.Subscribe(entry => AuditTrail.Add(entry));
        }

        public string RequestedDicomTagInput
        {
            get => _requestedDicomTagInput;
            set => this.RaiseAndSetIfChanged(ref _requestedDicomTagInput, value);
        }

        public string PatientId
        {
            get => _patientId;
            set => this.RaiseAndSetIfChanged(ref _patientId, value);
        }

        public string AccessionNumber
        {
            get => _accessionNumber;
            set => this.RaiseAndSetIfChanged(ref _accessionNumber, value);
        }

        public string StudyInstanceUid
        {
            get => _studyInstanceUid;
            set => this.RaiseAndSetIfChanged(ref _studyInstanceUid, value);
        }

        public DicomQueryRetrieveLevel RetrieveLevel
        {
            get => _retrieveLevel;
            set
            {
                this.RaiseAndSetIfChanged(ref _retrieveLevel, value);
                this.RaisePropertyChanged(nameof(IsPatientLevel));
                this.RaisePropertyChanged(nameof(IsStudyLevel));
                this.RaisePropertyChanged(nameof(IsSeriesLevel));
            }
        }

        public DicomRequestType RequestType
        {
            get => _requestType;
            set
            {
                this.RaiseAndSetIfChanged(ref _requestType, value);
                this.RaisePropertyChanged(nameof(IsStandardPatientRequest));
                this.RaisePropertyChanged(nameof(IsStandardStudyRequest));
                this.RaisePropertyChanged(nameof(IsStandardSeriesRequest));
                this.RaisePropertyChanged(nameof(IsCustomRequest));
            }
        }

        public string PageSize
        {
            get => _pageSize;
            set => this.RaiseAndSetIfChanged(ref _pageSize, value);
        }

        public bool IsPatientLevel
        {
            get => RetrieveLevel == DicomQueryRetrieveLevel.Patient;
            set => RetrieveLevel = DicomQueryRetrieveLevel.Patient;
        }

        public bool IsStudyLevel
        {
            get => RetrieveLevel == DicomQueryRetrieveLevel.Study;
            set => RetrieveLevel = DicomQueryRetrieveLevel.Study;
        }

        public bool IsSeriesLevel
        {
            get => RetrieveLevel == DicomQueryRetrieveLevel.Series;
            set => RetrieveLevel = DicomQueryRetrieveLevel.Series;
        }

        public bool IsStandardPatientRequest
        {
            get => RequestType == DicomRequestType.StandardPatient;
            set => RequestType = DicomRequestType.StandardPatient;
        }

        public bool IsStandardStudyRequest
        {
            get => RequestType == DicomRequestType.StandardStudy;
            set => RequestType = DicomRequestType.StandardStudy;
        }

        public bool IsStandardSeriesRequest
        {
            get => RequestType == DicomRequestType.StandardSeries;
            set => RequestType = DicomRequestType.StandardSeries;
        }

        public bool IsCustomRequest
        {
            get => RequestType == DicomRequestType.Custom;
            set => RequestType = DicomRequestType.Custom;
        }

        public ObservableCollection<DicomTagItem> RequestedDicomTags { get; } = new();
        public ObservableCollection<DicomTagItem> SelectedRequestedDicomTags { get; set; } = new();
        public ObservableCollection<AuditTrailEntry> AuditTrail { get; set; } = new();

        public ReactiveCommand<Unit, DicomTagItem>? AddRequestedDicomTag { get; protected set; }
        public ReactiveCommand<Unit, Unit>? RemoveRequestedDicomTags { get; protected set; }
        public ReactiveCommand<Unit, Unit>? ClearRequestedDicomTags { get; protected set; }
        public ReactiveCommand<Unit, Unit>? ArrangeStandardPatientQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit>? ArrangeStandardStudyQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit>? ArrangeStandardSeriesQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit>? ArrangeCustomQuery { get; protected set; }
        public ReactiveCommand<Unit, DicomQueryInputs>? StartQuery { get; protected set; }
        public ReactiveCommand<Unit, DicomQueryInputs>? StartPagedQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit>? Test { get; protected set; }

        private void ConfigureAddRequestedDicomTagButton()
        {
            var enabledObservable = this.WhenAnyValue(vm => vm.RequestedDicomTagInput, i => !i.IsNullOrEmpty());
            AddRequestedDicomTag = ReactiveCommand.Create(() =>
            {
                var newItem = new DicomTagItem(RequestedDicomTagInput);
                RequestedDicomTagInput = string.Empty;

                return newItem;
            }, enabledObservable);
        }

        private void ConfigureRemoveRequestedDicomTagsButton()
        {
            RemoveRequestedDicomTags = ReactiveCommand.Create(() => RequestedDicomTags.RemoveMany(SelectedRequestedDicomTags));
        }

        private void ConfigureClearRequestedDicomTagsButton()
        {
            ClearRequestedDicomTags = ReactiveCommand.Create(() => RequestedDicomTags.Clear());
        }

        private void ConfigureArrangeStandardPatientQueryButton()
        {
            ArrangeStandardPatientQuery = ReactiveCommand.Create(() =>
            {
                RetrieveLevel = DicomQueryRetrieveLevel.Patient;
                RequestedDicomTags.Clear();
                RequestedDicomTags.AddRange(new[]
                {
                    new DicomTagItem(nameof(DicomTag.PatientID)),
                    new DicomTagItem(nameof(DicomTag.PatientName)),
                    new DicomTagItem(nameof(DicomTag.IssuerOfPatientID)),
                    new DicomTagItem(nameof(DicomTag.PatientSex)),
                    new DicomTagItem(nameof(DicomTag.PatientBirthDate))
                });
            });
        }

        private void ConfigureArrangeStandardStudyQueryButton()
        {
            ArrangeStandardStudyQuery = ReactiveCommand.Create(() =>
            {
                RetrieveLevel = DicomQueryRetrieveLevel.Study;
                RequestedDicomTags.Clear();
                RequestedDicomTags.AddRange(new[]
                {
                    new DicomTagItem(nameof(DicomTag.PatientID)),
                    new DicomTagItem(nameof(DicomTag.PatientName)),
                    new DicomTagItem(nameof(DicomTag.IssuerOfPatientID)),
                    new DicomTagItem(nameof(DicomTag.PatientSex)),
                    new DicomTagItem(nameof(DicomTag.PatientBirthDate)),
                    new DicomTagItem(nameof(DicomTag.StudyInstanceUID)),
                    new DicomTagItem(nameof(DicomTag.ModalitiesInStudy)),
                    new DicomTagItem(nameof(DicomTag.StudyID)),
                    new DicomTagItem(nameof(DicomTag.AccessionNumber)),
                    new DicomTagItem(nameof(DicomTag.StudyDate)),
                    new DicomTagItem(nameof(DicomTag.StudyTime)),
                    new DicomTagItem(nameof(DicomTag.StudyDescription)),
                    new DicomTagItem(nameof(DicomTag.NumberOfStudyRelatedSeries)),
                    new DicomTagItem(nameof(DicomTag.NumberOfStudyRelatedInstances))
                });
            });
        }

        private void ConfigureArrangeStandardSeriesQueryButton()
        {
            ArrangeStandardSeriesQuery = ReactiveCommand.Create(() =>
            {
                RetrieveLevel = DicomQueryRetrieveLevel.Series;
                RequestedDicomTags.Clear();
                RequestedDicomTags.AddRange(new[]
                {
                    new DicomTagItem(nameof(DicomTag.StudyInstanceUID)),
                    new DicomTagItem(nameof(DicomTag.SeriesInstanceUID)),
                    new DicomTagItem(nameof(DicomTag.SeriesNumber)),
                    new DicomTagItem(nameof(DicomTag.SeriesDescription)),
                    new DicomTagItem(nameof(DicomTag.Modality)),
                    new DicomTagItem(nameof(DicomTag.SeriesDate)),
                    new DicomTagItem(nameof(DicomTag.SeriesTime)),
                    new DicomTagItem(nameof(DicomTag.NumberOfSeriesRelatedInstances))
                });
            });
        }

        private void ConfigureArrangeCustomQueryButton()
        {
            ArrangeCustomQuery = ReactiveCommand.Create(() =>
            {
            });
        }

        private void ConfigureStartQueryButton()
        {
            var enableObservable = this.WhenAnyValue(
                vm => vm.RetrieveLevel, vm => vm.RequestType, vm => vm.PatientId, vm => vm.StudyInstanceUid, vm => vm.AccessionNumber,
                (retrieveLevel, requestType, patiendId, studyInstanceUid, accessionNumber) =>
                    retrieveLevel != DicomQueryRetrieveLevel.NotApplicable && retrieveLevel != DicomQueryRetrieveLevel.Image &&
                    requestType != DicomRequestType.None &&
                    (patiendId.Any() || studyInstanceUid.Any() || accessionNumber.Any())
            );
            StartQuery = ReactiveCommand.Create(() =>
            {
                AuditTrail.Clear();

                return new DicomQueryInputs(RequestType, RetrieveLevel, PatientId, StudyInstanceUid, AccessionNumber, RequestedDicomTags, false, null);
            }, enableObservable);
        }

        private void ConfigureStartPagedQueryButton()
        {
            var enableObservable = this.WhenAnyValue(
                vm => vm.RetrieveLevel, vm => vm.RequestType, vm => vm.PatientId, vm => vm.StudyInstanceUid, vm => vm.AccessionNumber, vm => vm.PageSize,
                (retrieveLevel, requestType, patiendId, studyInstanceUid, accessionNumber, pageSize) =>
                    retrieveLevel != DicomQueryRetrieveLevel.NotApplicable && retrieveLevel != DicomQueryRetrieveLevel.Image &&
                    requestType != DicomRequestType.None &&
                    (patiendId.Any() || studyInstanceUid.Any() || accessionNumber.Any()) &&
                    pageSize.Any()
            );
            StartPagedQuery = ReactiveCommand.Create(() =>
            {
                AuditTrail.Clear();

                return new DicomQueryInputs(RequestType, RetrieveLevel, PatientId, StudyInstanceUid, AccessionNumber, RequestedDicomTags, false, null);
            }, enableObservable);
        }

        private void ConfigureTestButton()
        {
            Test = ReactiveCommand.Create(() =>
            {
                RetrieveLevel = DicomQueryRetrieveLevel.Series;
                RequestType = DicomRequestType.StandardSeries;
                StudyInstanceUid = "1.2.276.0.33.1.0.4.192.168.56.148.20200331.1192945.88622.2";
            });
        }
    }
}
