using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Common.Extensions;
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
        private DicomRequestType _dicomRequestType;
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

        public DicomRequestType DicomRequestType
        {
            get => _dicomRequestType;
            set
            {
                this.RaiseAndSetIfChanged(ref _dicomRequestType, value);
                this.RaisePropertyChanged(nameof(IsStandardPatientRequest));
                this.RaisePropertyChanged(nameof(IsStandardStudyRequest));
                this.RaisePropertyChanged(nameof(IsStandardSeriesRequest));
                this.RaisePropertyChanged(nameof(IsCustomRequest));
            }
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
            get => DicomRequestType == DicomRequestType.StandardPatient;
            set => DicomRequestType = DicomRequestType.StandardPatient;
        }

        public bool IsStandardStudyRequest
        {
            get => DicomRequestType == DicomRequestType.StandardStudy;
            set => DicomRequestType = DicomRequestType.StandardStudy;
        }

        public bool IsStandardSeriesRequest
        {
            get => DicomRequestType == DicomRequestType.StandardSeries;
            set => DicomRequestType = DicomRequestType.StandardSeries;
        }

        public bool IsCustomRequest
        {
            get => DicomRequestType == DicomRequestType.Custom;
            set => DicomRequestType = DicomRequestType.Custom;
        }

        public ObservableCollection<DicomTagItem> RequestedDicomTags { get; } = new();
        public ObservableCollection<DicomTagItem> SelectedRequestedDicomTags { get; set; } = new();
        public ObservableCollection<AuditTrailEntry> AuditTrail { get; set; } = new();

        public ReactiveCommand<Unit, DicomTagItem>? AddRequestedDicomTag { get; protected set; }
        public ReactiveCommand<Unit, Unit>? RemoveRequestedDicomTags { get; protected set; }
        public ReactiveCommand<Unit, Unit>? ClearRequestedDicomTags { get; protected set; }
        public ReactiveCommand<Unit, Unit> ArrangeStandardPatientQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit> ArrangeStandardStudyQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit> ArrangeStandardSeriesQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit> ArrangeCustomQuery { get; protected set; }
        public ReactiveCommand<Unit, DicomQueryInputs> StartQuery { get; protected set; }

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
            });
        }

        private void ConfigureArrangeStandardStudyQueryButton()
        {
            ArrangeStandardStudyQuery = ReactiveCommand.Create(() =>
            {
            });
        }

        private void ConfigureArrangeStandardSeriesQueryButton()
        {
            ArrangeStandardSeriesQuery = ReactiveCommand.Create(() =>
            {
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
            StartQuery = ReactiveCommand.Create(() =>
            {
                AuditTrail.Clear();

                return new DicomQueryInputs(DicomRequestType, RetrieveLevel, PatientId, StudyInstanceUid, AccessionNumber, RequestedDicomTags, false, null);
            });
        }
    }
}
