using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DicomCrawler.Enums;
using DicomCrawler.Models;

namespace DicomCrawler.ViewModels
{
    public class DicomQueryViewModel : INotifyPropertyChanged
    {
        private RetrieveLevel _retrieveLevel;
        private DicomQueryParameterViewModel _parameter;
        private string _dicomTagInput;
        private ISet<string> _dicomTags;

        public DicomQueryViewModel()
        {
            _parameter = new DicomQueryParameterViewModel();
            _dicomTags = new HashSet<string>();
        }

        public DicomQueryViewModel(DicomQueryViewModel dicomQuery)
        {
            _retrieveLevel = dicomQuery.RetrieveLevel;
            _parameter = new DicomQueryParameterViewModel(dicomQuery.Parameter);
            _dicomTagInput = dicomQuery.DicomTagInput;
            _dicomTags = new HashSet<string>(dicomQuery.DicomTags);
        }

        public RetrieveLevel RetrieveLevel
        {
            get => _retrieveLevel;
            set
            {
                if (_retrieveLevel != value)
                {
                    _retrieveLevel = value;
                    OnPropertyChanged();
                }
            }
        }

        public DicomQueryParameterViewModel Parameter
        {
            get => _parameter;
            set
            {
                if (_parameter != value)
                {
                    _parameter = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DicomTagInput
        {
            get => _dicomTagInput;
            set
            {
                if (_dicomTagInput != null)
                {
                    _dicomTagInput = value;
                    OnPropertyChanged();
                }
            }
        }

        public ISet<string> DicomTags
        {
            get => _dicomTags;
            set
            {
                if (!_dicomTags.Equals(value))
                {
                    _dicomTags = value;
                    OnPropertyChanged();
                }
            }
        }

        public static event EventHandler<ViewModelEventArgs<DicomQueryViewModel>> ViewModelChanged;

        public static void OnViewModelChanged(DicomQueryViewModel newViewModel) =>
            ViewModelChanged?.Invoke(null, new ViewModelEventArgs<DicomQueryViewModel>(newViewModel));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
