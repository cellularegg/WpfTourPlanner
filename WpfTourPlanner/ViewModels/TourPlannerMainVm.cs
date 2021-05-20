using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
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
        private string _tourDistance;
        private string _tourName;
        private string _tourDescription;

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
                if ((_currentTour != value))
                {
                    _currentTour = value;
                    RaisePropertyChangedEvent(nameof(CurrentTour));
                    if (value == null)
                    {
                        TourName = string.Empty;
                        TourDescription = string.Empty;
                        TourDistance = string.Empty;
                    }
                    else
                    {
                        TourName = _currentTour.Name;
                        TourDescription = _currentTour.Description;
                        TourDistance = _currentTour.DistanceInKm.ToString();
                    }
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
                    return _tourName;
                }

                return string.Empty;
            }
            set
            {
                if (value != null && value != _tourName)
                {
                    _tourName = value;
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
                    return _tourDescription;
                }

                return string.Empty;
            }
            set
            {
                if (value != null && value != _tourDescription)
                {
                    _tourDescription = value;
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
                    return _tourDistance;
                }

                return string.Empty;
            }
            set
            {
                if (value != null && _tourDistance != value)
                {
                    _tourDistance = value;
                }

                RaisePropertyChangedEvent(nameof(TourDistance));
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
                    Tour newlyCreatedTour = _tourPlannerManager.CreateTour(CurrentTour.Name + " Copy",
                        CurrentTour.Description,
                        CurrentTour.Information, CurrentTour.DistanceInKm);
                    Tours.Add(newlyCreatedTour);
                    TourLog newlyCreatedLog;
                    foreach (TourLog log in CurrentTour.Logs)
                    {
                        newlyCreatedLog = _tourPlannerManager.CreateTourLog(log.Report + " Copy", log.LogDateTime,
                            log.TotalTimeInH,
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
                try
                {
                    double distance = Double.Parse(TourDistance);
                    Tour updatedTour = _tourPlannerManager.UpdateTour(CurrentTour.Id, TourName, TourDescription,
                        CurrentTour.Information, distance);
                    Debug.WriteLine($"Updated Tour: {updatedTour}");
                    if (updatedTour == null)
                    {
                        MessageBox.Show("Error Tour Could not be updated", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        // TODO custom exception??
                    }
                    else
                    {
                        Tours.Clear();
                        FillTourList();
                        CurrentTour = null;
                    }
                }
                catch (OverflowException ex)
                {
                    MessageBox.Show($"Error Tour Could not be updated{Environment.NewLine}{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteLine(ex);
                }
                catch (FormatException ex)
                {
                    MessageBox.Show($"Error Tour Could not be updated{Environment.NewLine}{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteLine(ex);
                }
                catch (ArgumentNullException ex)
                {
                    MessageBox.Show($"Error Tour Could not be updated{Environment.NewLine}{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteLine(ex);
                }
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
            return CurrentTour != null && _tourDescription != null && _tourName != null &&
                   !String.IsNullOrWhiteSpace(_tourName) && !String.IsNullOrWhiteSpace(_tourDescription) &&
                   !String.IsNullOrWhiteSpace(_tourDistance) && Double.TryParse(_tourDistance, out double val) &&
                   val > 0;
        }
    }
}