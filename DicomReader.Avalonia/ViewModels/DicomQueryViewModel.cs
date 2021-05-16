using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Common.Extensions;
using DicomReader.Avalonia.Enums;
using DicomReader.Avalonia.Helper;
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
        private DicomRetrieveType _retrieveType;
        private ObservableAsPropertyHelper<bool>? _canExecuteQueryHelper;
        private ObservableAsPropertyHelper<bool>? _canExecutePagedQueryHelper;
        private DicomRetrieveLevel _retrieveLevel;
        private DicomRequestType _requestType;
        private int _page = 1;

        public DicomQueryViewModel()
        {
            ConfigureAddRequestedDicomTagButton();
            ConfigureRemoveRequestedDicomTagsButton();
            ConfigureStartQueryButton();
            ConfigureStartPagedQueryButton();
            DefineEnabledProperties();
            MainWindowViewModel.NextPageRequested += (_, _) => HandleNextPageRequested();

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

        public DicomRetrieveType RetrieveType
        {
            get => _retrieveType;
            set
            {
                if (value == default) return;

                this.RaiseAndSetIfChanged(ref _retrieveType, value);
            }
        }

        public DicomRequestType RequestType
        {
            get => _requestType;
            set => this.RaiseAndSetIfChanged(ref _requestType, value);
        }

        public string PageSize
        {
            get => _pageSize;
            set => this.RaiseAndSetIfChanged(ref _pageSize, value);
        }

        public ObservableCollection<DicomTagItem> RequestedDicomTags { get; } = new();
        public ObservableCollection<DicomTagItem> SelectedRequestedDicomTags { get; } = new();
        public ObservableCollection<AuditTrailEntry> AuditTrail { get; } = new();

        public ReactiveCommand<Unit, Unit>? AddRequestedDicomTag { get; protected set; }
        public ReactiveCommand<Unit, Unit>? RemoveRequestedDicomTags { get; protected set; }
        public ReactiveCommand<Unit, DicomQueryInputs>? StartQuery { get; protected set; }
        public ReactiveCommand<Unit, DicomQueryInputs>? StartPagedQuery { get; protected set; }

        public bool CanExecuteQuery => _canExecuteQueryHelper?.Value ?? false;
        public bool CanExecutePagedQuery => _canExecutePagedQueryHelper?.Value ?? false;
        public bool CanRemoveRequestedDicomTags => SelectedRequestedDicomTags.Any();

        public void InsertTestEntries()
        {
            RetrieveLevel = DicomRetrieveLevel.Series;
            RetrieveType = DicomRetrieveType.StandardSeries;
            RequestType = DicomRequestType.CFind;
            StudyInstanceUid = "1.3.6.1.4.1.24930.2.64870016549187.1743696";
        }

        public void AddPatientTags() => AddRequestedFields(DicomTagItemLists.Patients);

        public void AddStudyTags() => AddRequestedFields(DicomTagItemLists.Study);

        public void AddSeriesTags() => AddRequestedFields(DicomTagItemLists.Series);

        public void AddPatientStudiesTags() => AddRequestedFields(DicomTagItemLists.PatientStudies);

        public void AddDeviceInfosTags() => AddRequestedFields(DicomTagItemLists.DeviceInfos);

        public void AddPatientExtendedTags() => AddRequestedFields(DicomTagItemLists.PatientsExtended);

        public void AddSeriesExtendedTags() => AddRequestedFields(DicomTagItemLists.SeriesExtended);

        public void ClearQueryInput()
        {
            PatientId = string.Empty;
            AccessionNumber = string.Empty;
            StudyInstanceUid = string.Empty;
            _retrieveLevel = DicomRetrieveLevel.None;
            this.RaisePropertyChanged(nameof(RetrieveLevel));
            _retrieveType = DicomRetrieveType.None;
            this.RaisePropertyChanged(nameof(RetrieveType));
            AuditTrail.Clear();
        }

        public void ClearRequestedDicomTags() => RequestedDicomTags.Clear();

        public void ArrangeStandardPatientQuery()
        {
            RetrieveLevel = DicomRetrieveLevel.Patient;
            RequestType = DicomRequestType.CFind;
            RequestedDicomTags.Clear();
            RequestedDicomTags.AddRange(DicomTagItemLists.StandardPatient);
        }

        public void ArrangeStandardStudyQuery()
        {
            RetrieveLevel = DicomRetrieveLevel.Study;
            RequestType = DicomRequestType.CFind;
            RequestedDicomTags.Clear();
            RequestedDicomTags.AddRange(DicomTagItemLists.StandardStudy);
        }

        public void ArrangeStandardSeriesQuery()
        {
            RetrieveLevel = DicomRetrieveLevel.Series;
            RequestType = DicomRequestType.CFind;
            RequestedDicomTags.Clear();
            RequestedDicomTags.AddRange(DicomTagItemLists.StandardSeries);
        }

        private void ConfigureAddRequestedDicomTagButton()
        {
            var enabledObservable = this.WhenAnyValue(vm => vm.RequestedDicomTagInput, i => !i.IsNullOrEmpty());
            AddRequestedDicomTag = ReactiveCommand.Create(() =>
            {
                RequestedDicomTags.Add(new DicomTagItem(RequestedDicomTagInput));
                RequestedDicomTagInput = string.Empty;
            }, enabledObservable);
        }

        private void ConfigureRemoveRequestedDicomTagsButton()
        {
            var enabledObservable = this.WhenAnyValue(vm => vm.CanRemoveRequestedDicomTags).DistinctUntilChanged();
            RemoveRequestedDicomTags = ReactiveCommand.Create(() => RequestedDicomTags.RemoveMany(SelectedRequestedDicomTags), enabledObservable);
        }

        private void ConfigureStartQueryButton()
        {
            var enabledObservable = this.WhenAnyValue(vm => vm.CanExecuteQuery).DistinctUntilChanged();
            StartQuery = ReactiveCommand.Create(() => ExecuteQuery(false), enabledObservable);
        }

        private void ConfigureStartPagedQueryButton()
        {
            var enabledObservable = this.WhenAnyValue(vm => vm.CanExecutePagedQuery).DistinctUntilChanged();
            StartPagedQuery = ReactiveCommand.Create(() =>
            {
                _page = 1;

                return ExecuteQuery(true);
            }, enabledObservable);
        }

        private DicomQueryInputs ExecuteQuery(bool isPagedQuery)
        {
            AuditTrail.Clear();

            return new DicomQueryInputs(RetrieveType, RetrieveLevel, RequestType, PatientId, StudyInstanceUid, AccessionNumber, RequestedDicomTags,
                isPagedQuery, PageSize, _page);
        }

        private void DefineEnabledProperties()
        {
            // can execute query
            this.WhenAnyValue(
                vm => vm.RetrieveLevel, vm => vm.RetrieveType, vm => vm.RequestType, vm => vm.PatientId, vm => vm.StudyInstanceUid, vm => vm.AccessionNumber,
                (retrieveLevel, retrieveType, requestType, patiendId, studyInstanceUid, accessionNumber) =>
                    retrieveLevel != default &&
                    requestType != default &&
                    retrieveType != default &&
                    (patiendId.Any() || studyInstanceUid.Any() || accessionNumber.Any())
            ).ToProperty(this, vm => vm.CanExecuteQuery, out _canExecuteQueryHelper);

            // can execute paged query
            this.WhenAnyValue(vm => vm.CanExecuteQuery, vm => vm.PageSize, (canExecute, pageSize) => canExecute && !pageSize.IsNullOrEmpty())
                .ToProperty(this, vm => vm.CanExecutePagedQuery, out _canExecutePagedQueryHelper);
        }

        private void AddRequestedFields(IEnumerable<DicomTagItem> dicomTagItems) => RequestedDicomTags.AddRange(dicomTagItems.Except(RequestedDicomTags));

        private void HandleNextPageRequested()
        {
            _page++;
            StartPagedQuery?.Execute();
        }
    }
}
