using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        public ICommand DuplicateTourCommand { get; }

        public ICommand DuplicateTourLogCommand { get; }

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

            this.DuplicateTourCommand = new RelayCommand(o =>
            {
                if (_currentTour != null)
                {
                    Tour newlyCreatedTour = _tourPlannerManager.CreateTour(CurrentTour.Name + " Copy", CurrentTour.Description,
                        CurrentTour.Information, CurrentTour.DistanceInKm);
                    Tours.Add(newlyCreatedTour);
                    TourLog newlyCreatedLog;
                    foreach (TourLog log in CurrentTour.Logs)
                    {
                        newlyCreatedLog = _tourPlannerManager.CreateTourLog(log.Report + " Copy", log.LogDateTime, log.TotalTimeInH, 
                            log.Rating, log.HeartRate, log.AverageSpeedInKmH, log.TemperatureInC, log.Breaks, log.Steps, 
                            newlyCreatedTour);
                        newlyCreatedTour.Logs.Add(newlyCreatedLog);
                    }
                }
            }, new Predicate<object>(CanExecuteDuplicateTour));

            this.DuplicateTourLogCommand = new RelayCommand(
                o =>
                {
                    TourLog log = _tourPlannerManager.CreateTourLog(CurrentLog.Report + " Copy", CurrentLog.LogDateTime,
                        CurrentLog.TotalTimeInH, CurrentLog.Rating, CurrentLog.HeartRate, CurrentLog.AverageSpeedInKmH,
                        CurrentLog.TemperatureInC, CurrentLog.Breaks, CurrentLog.Steps, CurrentTour);
                    CurrentTour.Logs.Add(log);
                }, new Predicate<object>(CanExecuteDuplicateTourLog));

            this.UpdateTourCommand = new RelayCommand(o =>
            {
                // TODO Actually update!!!
                Debug.WriteLine("Update Tour");
            }, new Predicate<object>(CanExecuteUpdateTour));

            FillTourList();
        }

        private bool CanExecuteDuplicateTourLog(object obj)
        {
            return CurrentLog != null && CurrentTour != null;
        }

        private void FillTourList()
        {
            foreach (Tour tour in _tourPlannerManager.GetTours())
            {
                Tours.Add(tour);
            }
        }

        private bool CanExecuteDuplicateTour(object param)
        {
            return CurrentTour != null;
        }

        private bool CanExecuteUpdateTour(object param)
        {
            return !String.IsNullOrWhiteSpace(CurrentTour?.Name) &&
                   !String.IsNullOrWhiteSpace(CurrentTour?.Description) && CurrentTour?.DistanceInKm > 0;
        }
    }
}