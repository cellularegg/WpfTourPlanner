using System;
using System.Diagnostics;
using System.Windows.Input;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models;
using WpfTourPlanner.Stores;
using WpfTourPlanner.Util;

namespace WpfTourPlanner.ViewModels
{
    public class AddTourLogViewModel : ViewModelBase
    {
        private ITourPlannerManager _tourPlannerManager;
        private TourLog _logToEdit;

        private string _report;
        private DateTime _logDateTime;
        private string _totalTimeInH;
        private string _heartRate;
        private string _averageSpeedInKmH;
        private string _temperatureInC;
        private string _breaks;
        private string _steps;
        private string _rating;

        public string TourHeaderLabel => _logToEdit == null
            ? $"Add new Tour Log for Tour: \"{CurrentTour.Name}\""
            : $"Editing Tour Log for Tour: \"{CurrentTour.Name}\"";

        public string SaveButtonLabel => _logToEdit == null ? " ➕ Add " : " 💾 Save ";

        public string Report
        {
            get => _report;
            set
            {
                if (_report != value)
                {
                    _report = value;
                    RaisePropertyChangedEvent(nameof(Report));
                }
            }
        }

        public string LogDateTime => _logDateTime.ToString();

        public string TotalTimeInH
        {
            get => _totalTimeInH;
            set
            {
                if (_totalTimeInH != value)
                {
                    _totalTimeInH = value;
                    RaisePropertyChangedEvent(nameof(TotalTimeInH));
                }
            }
        }

        public string HeartRate
        {
            get => _heartRate;
            set
            {
                if (_heartRate != value)
                {
                    _heartRate = value;
                    RaisePropertyChangedEvent(nameof(HeartRate));
                }
            }
        }

        public string AverageSpeedInKmH
        {
            get => _averageSpeedInKmH;
            set
            {
                if (_averageSpeedInKmH != value)
                {
                    _averageSpeedInKmH = value;
                    RaisePropertyChangedEvent(nameof(AverageSpeedInKmH));
                }
            }
        }

        public string TemperatureInC
        {
            get => _temperatureInC;
            set
            {
                if (_temperatureInC != value)
                {
                    _temperatureInC = value;
                    RaisePropertyChangedEvent(nameof(TemperatureInC));
                }
            }
        }

        public string Breaks
        {
            get => _breaks;
            set
            {
                if (_breaks != value)
                {
                    _breaks = value;
                    RaisePropertyChangedEvent(nameof(Breaks));
                }
            }
        }

        public string Steps
        {
            get => _steps;
            set
            {
                if (_steps != value)
                {
                    _steps = value;
                    RaisePropertyChangedEvent(nameof(Steps));
                }
            }
        }

        public string Rating
        {
            get => _rating;
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    RaisePropertyChangedEvent(nameof(Rating));
                }
            }
        }

        public ICommand NavigateHomeCommand { get; }

        public ICommand SaveTourLogCommand { get; }
        public Tour CurrentTour { get; }


        public AddTourLogViewModel(ITourPlannerManager tourPlannerManager, NavigationStore navigationStore,
            Tour currentTour, TourLog logToEdit = null)
        {
            _tourPlannerManager = tourPlannerManager;
            CurrentTour = currentTour;
            _logToEdit = logToEdit;
            if (_logToEdit == null)
            {
                Report = string.Empty;
                _logDateTime = DateTime.Now;
                TotalTimeInH = string.Empty;
                HeartRate = string.Empty;
                AverageSpeedInKmH = string.Empty;
                TemperatureInC = string.Empty;
                Breaks = string.Empty;
                Steps = string.Empty;
                Rating = string.Empty;
            }

            if (_logToEdit != null)
            {
                Report = _logToEdit.Report;
                _logDateTime = _logToEdit.LogDateTime;
                TotalTimeInH = _logToEdit.TotalTimeInH.ToString();
                HeartRate = _logToEdit.HeartRate.ToString();
                AverageSpeedInKmH = _logToEdit.AverageSpeedInKmH.ToString();
                TemperatureInC = _logToEdit.TemperatureInC.ToString();
                Breaks = _logToEdit.Breaks.ToString();
                Steps = _logToEdit.Steps.ToString();
                Rating = _logToEdit.Rating.ToString();
            }

            NavigateHomeCommand = new RelayCommand(o =>
            {
                navigationStore.CurrentViewModel = new HomeViewModel(_tourPlannerManager, navigationStore);
            });
            SaveTourLogCommand = new RelayCommand(o =>
            {
                TourLog result = AddOrUpdateTourLog();
                if (result == null)
                {
                    UtilMethods.ShowErrorMsgBox("Error creating Tour. Please check the logs!");
                    return;
                }

                // Navigate back "Home"
                navigationStore.CurrentViewModel = new HomeViewModel(_tourPlannerManager, navigationStore);
            }, CanExecuteAddOrUpdateTourLog);
        }

        private TourLog AddOrUpdateTourLog()
        {
            // Parse data Should not throw any exceptions because can execute checks if all values are valid!
            try
            {
                double totalTimeInH = Double.Parse(TotalTimeInH);
                double heartRate = Double.Parse(HeartRate);
                double averageSpeedInKmH = Double.Parse(AverageSpeedInKmH);
                double temperatureInC = Double.Parse(TemperatureInC);
                int breaks = Int32.Parse(Breaks);
                int steps = Int32.Parse(Steps);
                int rating = Int32.Parse(Rating);
                TourLog log = null;
                if (_logToEdit == null)
                {
                    log = _tourPlannerManager.CreateTourLog(Report, _logDateTime, totalTimeInH, rating,
                        heartRate, averageSpeedInKmH, temperatureInC, breaks, steps, CurrentTour);
                }

                if (_logToEdit != null)
                {
                    log = _tourPlannerManager.UpdateTourLog(_logToEdit.Id, Report, _logDateTime,
                        totalTimeInH, rating, heartRate, averageSpeedInKmH, temperatureInC, breaks, steps);
                }

                return log;
            }
            catch (ArgumentNullException e)
            {
                Debug.WriteLine(e);
                UtilMethods.ShowErrorMsgBox($"Error parsing string!{Environment.NewLine}{e.Message}");
                return null;
            }
            catch (FormatException e)
            {
                Debug.WriteLine(e);
                UtilMethods.ShowErrorMsgBox($"Error parsing string!{Environment.NewLine}{e.Message}");

                return null;
            }
        }

        public bool CanExecuteAddOrUpdateTourLog(object obj)
        {
            return !String.IsNullOrWhiteSpace(Report) && UtilMethods.IsValidNumber(TotalTimeInH, TypeCode.Double) &&
                   UtilMethods.IsValidNumber(HeartRate, TypeCode.Double) &&
                   UtilMethods.IsValidNumber(AverageSpeedInKmH, TypeCode.Double) &&
                   UtilMethods.IsValidNumber(Breaks, TypeCode.Int32) &&
                   UtilMethods.IsValidNumber(Steps, TypeCode.Int32) &&
                   UtilMethods.IsValidNumber(Rating, TypeCode.Int32, 0, 10);
        }
    }
}