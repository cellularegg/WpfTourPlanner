using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.ViewModels
{
    public class TourPlannerMainVm : ViewModelBase
    {
        private ITourPlannerManager _tourPlannerManager;
        private Tour _currentTour;
        private string _searchQuery;
        private TourLog _currentLog;

        public ICommand SearchCommand { get; }

        public ICommand ClearCommand { get; }

        public ICommand UpdateTourCommand { get; }

        // TODO add other commands
        public ObservableCollection<Tour> Tours { get; private set; }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    RaisePropertyChangedEvent(nameof(SearchQuery));
                }
            }
        }

        public Tour CurrentTour
        {
            get
            {
                Debug.WriteLine($"Get Current Tour {_currentTour}");
                return _currentTour;
            }
            set
            {
                if ((_currentTour != value) && (value != null))
                {
                    _currentTour = value;
                    RaisePropertyChangedEvent(nameof(CurrentTour));
                    RaisePropertyChangedEvent(nameof(TourName));
                    RaisePropertyChangedEvent(nameof(TourDescription));
                    RaisePropertyChangedEvent(nameof(TourDistance));
                }
            }
        }

        public TourLog CurrentLog
        {
            get => _currentLog;
            set
            {
                if ((_currentLog != value) && (value != null))
                {
                    _currentLog = value;
                    RaisePropertyChangedEvent(nameof(CurrentLog));
                }
            }
        }

        public string TourName
        {
            get
            {
                if (CurrentTour != null)
                {
                    Debug.WriteLine($"Current Tour Name: {CurrentTour.Name}");
                    return CurrentTour.Name;
                }

                return string.Empty;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value) && value != CurrentTour.Name)
                {
                    CurrentTour.Name = value;
                    RaisePropertyChangedEvent(nameof(TourName));
                }
            }
        }


        public string TourDescription
        {
            get
            {
                if (CurrentTour != null)
                {
                    return CurrentTour.Description;
                }

                return string.Empty;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value) && value != CurrentTour.Description)
                {
                    CurrentTour.Description = value;
                    RaisePropertyChangedEvent(nameof(TourDescription));
                }
            }
        }

        public string TourDistance
        {
            get
            {
                if (CurrentTour != null)
                {
                    return CurrentTour.DistanceInKm.ToString();
                }

                return string.Empty;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value) && CurrentTour.DistanceInKm.ToString() != value &&
                    int.TryParse(value, out int val))
                {
                    CurrentTour.DistanceInKm = val;
                    RaisePropertyChangedEvent(nameof(TourDistance));
                }
            }
        }

        public TourPlannerMainVm(ITourPlannerManager tourPlannerManager)
        {
            _tourPlannerManager = tourPlannerManager;
            Tours = new ObservableCollection<Tour>();

            this.SearchCommand = new RelayCommand(o =>
            {
                IEnumerable<Tour> tours = _tourPlannerManager.Search(_searchQuery);
                Tours.Clear();
                foreach (Tour tour in tours)
                {
                    Tours.Add(tour);
                }
            });

            this.ClearCommand = new RelayCommand(o =>
            {
                Tours.Clear();
                SearchQuery = string.Empty;
                FillTourList();
            });

            this.UpdateTourCommand = new RelayCommand(o =>
            {
                // TODO Actually update!!!
                Debug.WriteLine("Update Tour");
            }, new Predicate<object>(IsInputValid));

            FillTourList();
        }

        private void FillTourList()
        {
            foreach (Tour tour in _tourPlannerManager.GetTours())
            {
                Tours.Add(tour);
            }
        }

        public bool IsInputValid(object param)
        {
            return !String.IsNullOrWhiteSpace(CurrentTour?.Name) &&
                   !String.IsNullOrWhiteSpace(CurrentTour?.Description) && CurrentTour?.DistanceInKm > 0;
        }
    }
}