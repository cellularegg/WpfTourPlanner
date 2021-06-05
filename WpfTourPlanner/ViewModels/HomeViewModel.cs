using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;
using Ookii.Dialogs.Wpf;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Commands;
using WpfTourPlanner.Models;
using WpfTourPlanner.Models.Exceptions;
using WpfTourPlanner.Stores;
using WpfTourPlanner.Util;

namespace WpfTourPlanner.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private ITourPlannerManager _tourPlannerManager;
        private Tour _currentTour;
        private string _searchQuery;
        private TourLog _currentLog;
        private string _tourDistance;
        private string _tourName;
        private string _tourDescription;

        public ICommand SearchCommand { get; }

        public ICommand ClearSearchCommand { get; }

        public ICommand UpdateTourCommand { get; }

        public ICommand DuplicateTourCommand { get; }

        public ICommand DuplicateTourLogCommand { get; }

        public ICommand DeleteTourCommand { get; }
        public ICommand DeleteTourLogCommand { get; }

        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }
        public ICommand ViewOnlineHelpCommand { get; }

        public ICommand NavigateAddTourCommand { get; }
        public ICommand NavigateAddTourLogCommand { get; }
        public ICommand NavigateEditTourLogCommand { get; }

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
                //Debug.WriteLine($"Get Current Tour {_currentTour}");
                return _currentTour;
            }
            set
            {
                if (_currentTour != value)
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
                if ((_currentLog != value))
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

        public HomeViewModel(ITourPlannerManager tourPlannerManager, NavigationStore navigationStore)
        {
            _tourPlannerManager = tourPlannerManager;
            Tours = new ObservableCollection<Tour>();
            _searchQuery = string.Empty;

            this.SearchCommand = new RelayCommand(SearchCommandMethod);

            this.ClearSearchCommand = new RelayCommand(ResetView, CanExecuteClearSearch);

            this.DuplicateTourCommand = new RelayCommand(DuplicateTourCommandMethod, CanExecuteDuplicateTour);

            this.DuplicateTourLogCommand = new RelayCommand(DuplicateTourLogCommandMethod, CanExecuteDuplicateTourLog);

            this.UpdateTourCommand = new RelayCommand(UpdateTourCommandMethod, CanExecuteUpdateTour);

            this.DeleteTourCommand = new RelayCommand(DeteTourCommandMethod, CanExecuteDeleteTour);

            this.DeleteTourLogCommand = new RelayCommand(DeleteTourLogCommandMethod, CanExecuteDeleteTourLog);

            this.ExportCommand = new RelayCommand(ExportCommandMethod);

            this.ImportCommand = new RelayCommand(ImportCommandMethod);

            this.GenerateTourReportCommand = new RelayCommand(GenerateTourReportCommandMethod,
                CanExecuteGenerateTourReport);

            this.GenerateSummaryReportCommand = new RelayCommand(GenerateSummaryReportCommandMethod);

            this.ViewOnlineHelpCommand = new RelayCommand(ViewOnlineHelpCommandMethod);

            this.NavigateAddTourCommand = new RelayCommand(o =>
            {
                navigationStore.CurrentViewModel = new AddTourViewModel(_tourPlannerManager, navigationStore);
            });
            this.NavigateAddTourLogCommand = new RelayCommand(
                o =>
                {
                    navigationStore.CurrentViewModel = new AddTourLogViewModel(_tourPlannerManager,
                        navigationStore, CurrentTour);
                }, CanExecuteNavigateAddTourLogViewModel);
            this.NavigateEditTourLogCommand = new RelayCommand(
                o =>
                {
                    navigationStore.CurrentViewModel = new AddTourLogViewModel(_tourPlannerManager, navigationStore,
                        CurrentTour, CurrentLog);
                }, CanExecuteNavigateEditTourLogViewModel);
            FillTourList();
        }

        private void SearchCommandMethod(object o)
        {
            IEnumerable<Tour> tours = _tourPlannerManager.Search(_searchQuery);
            CurrentTour = null;
            CurrentLog = null;
            Tours.Clear();
            foreach (Tour tour in tours)
            {
                Tours.Add(tour);
            }
        }

        private void DuplicateTourCommandMethod(object o)
        {
            if (_currentTour != null)
            {
                Tour duplicate = _tourPlannerManager.DuplicateTour(CurrentTour);
                Tours.Add(duplicate);
            }
        }

        private void DuplicateTourLogCommandMethod(object o)
        {
            TourLog log = _tourPlannerManager.CreateTourLog(CurrentLog.Report + " Copy", CurrentLog.LogDateTime,
                CurrentLog.TotalTimeInH, CurrentLog.Rating, CurrentLog.HeartRate, CurrentLog.AverageSpeedInKmH,
                CurrentLog.TemperatureInC, CurrentLog.Breaks, CurrentLog.Steps, CurrentTour);
            CurrentTour.Logs.Add(log);
        }

        private void UpdateTourCommandMethod(object o)
        {
            try
            {
                double distance = Double.Parse(TourDistance);
                Tour updatedTour = _tourPlannerManager.UpdateTour(CurrentTour.Id, TourName, TourDescription,
                    CurrentTour.Information, distance);
                Debug.WriteLine($"Updated Tour: {updatedTour}");
                if (updatedTour == null)
                {
                    UtilMethods.ShowErrorMsgBox("Error Tour Could not be updated");
                    // TODO custom exception??
                }
                else
                {
                    ResetView();
                }
            }
            catch (OverflowException ex)
            {
                UtilMethods.ShowErrorMsgBox($"Error Tour Could not be updated{Environment.NewLine}{ex.Message}");
                Debug.WriteLine(ex);
            }
            catch (FormatException ex)
            {
                UtilMethods.ShowErrorMsgBox($"Error Tour Could not be updated{Environment.NewLine}{ex.Message}");
                Debug.WriteLine(ex);
            }
            catch (ArgumentNullException ex)
            {
                UtilMethods.ShowErrorMsgBox($"Error Tour Could not be updated{Environment.NewLine}{ex.Message}");
                Debug.WriteLine(ex);
            }
        }

        private void ViewOnlineHelpCommandMethod(object o)
        {
            string url = "https://github.com/cellularegg/WpfTourPlanner/blob/main/README.md";
            Clipboard.SetText(url);
            MessageBox.Show(
                $"The Url of the documentation has been copied to your clipboard!{Environment.NewLine}" +
                $"URL:{Environment.NewLine}{url}", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GenerateSummaryReportCommandMethod(object o)
        {
            Debug.WriteLine("Generating summary report");
            string folderPath = OpenFolderSelectionDialog();
            if (folderPath != null)
            {
                _tourPlannerManager.GenerateSummaryReport(folderPath);
            }
        }

        private void GenerateTourReportCommandMethod(object o)
        {
            Debug.WriteLine("Generating tour report");
            string folderPath = OpenFolderSelectionDialog();
            if (folderPath != null)
            {
                _tourPlannerManager.GenerateTourReport(CurrentTour, folderPath);
            }
        }

        private void DeteTourCommandMethod(object o)
        {
            Debug.WriteLine("Delete tour!");
            int currentTourId = CurrentTour.Id;
            if (_tourPlannerManager.DeleteTour(currentTourId))
            {
                ResetView();
            }
            else
            {
                UtilMethods.ShowErrorMsgBox($"Error Tour with Id: {currentTourId} could not be deleted!");
            }
        }

        private void DeleteTourLogCommandMethod(object o)
        {
            if (_tourPlannerManager.DeleteTourLog(CurrentLog.Id))
            {
                CurrentTour.Logs.Remove(CurrentLog);
                CurrentLog = null;
            }
            else
            {
                UtilMethods.ShowErrorMsgBox($"Error TourLog with Id: {CurrentLog?.Id} could not be deleted!");
            }
        }

        private void ExportCommandMethod(object o)
        {
            Debug.WriteLine("Exporting");

            string folderPath = OpenFolderSelectionDialog();
            if (folderPath != null)
            {
                _tourPlannerManager.Export(folderPath);
            }
        }

        private void ImportCommandMethod(object o)
        {
            Debug.WriteLine("Importing...");
            VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.CheckFileExists = true;
            fileDialog.Filter = "Json files (*.json)|*.json";
            if (!VistaFileDialog.IsVistaFileDialogSupported)
            {
                UtilMethods.ShowErrorMsgBox(
                    "Because you are not using Windows Vista or later, the regular folder browser dialog will " +
                    "be used. Please use Windows Vista to see the new dialog.",
                    "Sample folder browser dialog");
            }

            bool? dialogResult = fileDialog.ShowDialog();
            if (dialogResult != null && (bool) dialogResult)
            {
                try
                {
                    _tourPlannerManager.Import(fileDialog.FileName);
                    this.ResetView();
                }
                catch (InvalidImportFileException e)
                {
                    Debug.WriteLine(e);
                    UtilMethods.ShowErrorMsgBox(e.Message);
                }

                // MessageBox.Show("The selected file was: " + fileDialog.FileName, "Sample folder browser dialog");
            }
        }

        private string OpenFolderSelectionDialog()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            // dialog.Description = "Please select a folder.";
            // dialog.UseDescriptionForTitle = true;
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                UtilMethods.ShowErrorMsgBox(
                    "Because you are not using Windows Vista or later, the regular folder browser dialog will be " +
                    "used. Please use Windows Vista to see the new dialog.",
                    "Sample folder browser dialog");
            }

            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult != null && (bool) dialogResult)
            {
                return dialog.SelectedPath;
            }

            return null;
        }


        public bool CanExecuteGenerateTourReport(object obj)
        {
            return CurrentTour != null;
        }

        public bool CanExecuteDeleteTourLog(object obj)
        {
            return CurrentLog != null && CurrentLog != null;
        }

        public bool CanExecuteClearSearch(object obj)
        {
            return !String.IsNullOrWhiteSpace(_searchQuery);
        }

        public bool CanExecuteDeleteTour(object obj)
        {
            return CurrentTour != null;
        }

        public bool CanExecuteDuplicateTourLog(object obj)
        {
            return CurrentLog != null && CurrentTour != null;
        }

        private void FillTourList()
        {
            Tours.Clear();
            try
            {
                foreach (Tour tour in _tourPlannerManager.GetTours())
                {
                    Tours.Add(tour);
                }
            }
            catch (DatabaseException e)
            {
                Debug.WriteLine(e);
                UtilMethods.ShowErrorMsgBox(e.Message);
                throw;
            }
        }

        private void ResetView(object param = null)
        {
            FillTourList();
            SearchQuery = String.Empty;
            CurrentTour = null;
            CurrentLog = null;
        }

        public bool CanExecuteDuplicateTour(object param)
        {
            return CurrentTour != null;
        }

        public bool CanExecuteUpdateTour(object param)
        {
            return CurrentTour != null && _tourDescription != null && _tourName != null &&
                   !String.IsNullOrWhiteSpace(_tourName) && !String.IsNullOrWhiteSpace(_tourDescription) &&
                   !String.IsNullOrWhiteSpace(_tourDistance) && Double.TryParse(_tourDistance, out double val) &&
                   val > 0;
        }

        public bool CanExecuteNavigateAddTourLogViewModel(object param)
        {
            return CurrentTour != null;
        }


        public bool CanExecuteNavigateEditTourLogViewModel(object obj)
        {
            return CurrentLog != null;
        }
    }
}