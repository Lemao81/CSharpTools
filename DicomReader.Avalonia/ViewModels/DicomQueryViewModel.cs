﻿using System;
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

        public DicomQueryViewModel()
        {
            ConfigureAddRequestedDicomTagButton();
            ConfigureRemoveRequestedDicomTagsButton();
            ConfigureClearRequestedDicomTagsButton();
            ConfigureArrangeStandardPatientQueryButton();
            ConfigureArrangeStandardStudyQueryButton();
            ConfigureArrangeStandardSeriesQueryButton();
            ConfigureStartQueryButton();

            AddRequestedDicomTag?.Subscribe(dicomTag => RequestedDicomTags.Add(dicomTag));
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

        public DicomRequestType DicomRequestType
        {
            get => _dicomRequestType;
            set => this.RaiseAndSetIfChanged(ref _dicomRequestType, value);
        }

        public DicomQueryRetrieveLevel RetrieveLevel { get; set; }
        public ObservableCollection<DicomTagItem> RequestedDicomTags { get; } = new();
        public ObservableCollection<DicomTagItem> SelectedRequestedDicomTags { get; set; } = new();

        public ReactiveCommand<Unit, DicomTagItem>? AddRequestedDicomTag { get; protected set; }
        public ReactiveCommand<Unit, Unit>? RemoveRequestedDicomTags { get; protected set; }
        public ReactiveCommand<Unit, Unit>? ClearRequestedDicomTags { get; protected set; }
        public ReactiveCommand<Unit, Unit> ArrangeStandardPatientQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit> ArrangeStandardStudyQuery { get; protected set; }
        public ReactiveCommand<Unit, Unit> ArrangeStandardSeriesQuery { get; protected set; }
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

        private void ConfigureStartQueryButton()
        {
            StartQuery = ReactiveCommand.Create(() =>
                new DicomQueryInputs(DicomRequestType, RetrieveLevel, PatientId, StudyInstanceUid, AccessionNumber, RequestedDicomTags, false, null));
        }
    }
}
