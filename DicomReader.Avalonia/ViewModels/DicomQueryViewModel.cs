using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Common.Extensions;
using Dicom;
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
        private ObservableAsPropertyHelper<bool>? _canExecuteQueryHelper;
        private ObservableAsPropertyHelper<bool>? _canExecutePagedQueryHelper;
        private DicomRetrieveLevel _retrieveLevel;

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
            DefineEnabledProperties();

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

        public DicomRetrieveLevel RetrieveLevel
        {
            get => _retrieveLevel;
            set
            {
                if (value == default) return;

                this.RaiseAndSetIfChanged(ref _retrieveLevel, value);
            }
        }

        public DicomRequestType RequestType
        {
            get => _requestType;
            set
            {
                if (value == default) return;

                this.RaiseAndSetIfChanged(ref _requestType, value);
            }
        }

        public string PageSize
        {
            get => _pageSize;
            set => this.RaiseAndSetIfChanged(ref _pageSize, value);
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

        public bool CanExecuteQuery => _canExecuteQueryHelper?.Value ?? false;
        public bool CanExecutePagedQuery => _canExecutePagedQueryHelper?.Value ?? false;

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
                RetrieveLevel = DicomRetrieveLevel.Patient;
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
                RetrieveLevel = DicomRetrieveLevel.Study;
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
                RetrieveLevel = DicomRetrieveLevel.Series;
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
            StartQuery = ReactiveCommand.Create(() =>
            {
                AuditTrail.Clear();

                return new DicomQueryInputs(RequestType, RetrieveLevel, PatientId, StudyInstanceUid, AccessionNumber, RequestedDicomTags, false,
                    null);
            }, this.WhenAnyValue(vm => vm.CanExecuteQuery).DistinctUntilChanged());
        }

        private void ConfigureStartPagedQueryButton()
        {
            StartPagedQuery = ReactiveCommand.Create(() =>
            {
                AuditTrail.Clear();

                return new DicomQueryInputs(RequestType, RetrieveLevel, PatientId, StudyInstanceUid, AccessionNumber, RequestedDicomTags, false,
                    null);
            }, this.WhenAnyValue(vm => vm.CanExecutePagedQuery).DistinctUntilChanged());
        }

        private void ConfigureTestButton()
        {
            Test = ReactiveCommand.Create(() =>
            {
                RetrieveLevel = DicomRetrieveLevel.Series;
                RequestType = DicomRequestType.StandardSeries;
                StudyInstanceUid = "1.2.276.0.33.1.0.4.192.168.56.148.20200331.1192945.88622.2";
            });
        }

        private void DefineEnabledProperties()
        {
            // can execute query
            this.WhenAnyValue(
                vm => vm.RetrieveLevel, vm => vm.RequestType, vm => vm.PatientId, vm => vm.StudyInstanceUid, vm => vm.AccessionNumber,
                (retrieveLevel, requestType, patiendId, studyInstanceUid, accessionNumber) =>
                    retrieveLevel != default &&
                    requestType != DicomRequestType.None &&
                    (patiendId.Any() || studyInstanceUid.Any() || accessionNumber.Any())
            ).ToProperty(this, vm => vm.CanExecuteQuery, out _canExecuteQueryHelper);

            // can execute paged query
            this.WhenAnyValue(vm => vm.CanExecuteQuery, vm => vm.PageSize, (canExecute, pageSize) => canExecute && !pageSize.IsNullOrEmpty())
                .ToProperty(this, vm => vm.CanExecutePagedQuery, out _canExecutePagedQueryHelper);
        }
    }
}
