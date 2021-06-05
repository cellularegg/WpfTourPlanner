using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Commands;
using WpfTourPlanner.Models;
using WpfTourPlanner.Stores;
using WpfTourPlanner.Util;

namespace WpfTourPlanner.ViewModels
{
    public class AddTourViewModel : ViewModelBase
    {
        private ITourPlannerManager _tourPlannerManager;
        private readonly NavigationStore _navigationStore;

        private string _tourName;
        private string _tourDescription;
        private string _tourFromLocation;
        private string _tourToLocation;
        private double _tourDistanceInKm;
        private bool _asyncCommandIsExecuting;
        private string _statusMessage;

        public bool AsyncCommandIsExecuting
        {
            get => _asyncCommandIsExecuting;
            set
            {
                _asyncCommandIsExecuting = value;
                RaisePropertyChangedEvent(nameof(AsyncCommandIsExecuting));
            }
        }

        public string TourName
        {
            get => _tourName;
            set
            {
                _tourName = value;
                RaisePropertyChangedEvent(nameof(TourName));
                AddTourCommand?.OnCanExecuteChanged();
            }
        }

        public string TourDescription
        {
            get => _tourDescription;
            set
            {
                _tourDescription = value;
                RaisePropertyChangedEvent(nameof(TourDescription));
                AddTourCommand?.OnCanExecuteChanged();
            }
        }

        public string TourFromLocation
        {
            get => _tourFromLocation;
            set
            {
                _tourFromLocation = value;
                RaisePropertyChangedEvent(nameof(TourFromLocation));
                CalculateDistanceCommand?.OnCanExecuteChanged();
                AddTourCommand?.OnCanExecuteChanged();
            }
        }

        public string TourToLocation
        {
            get => _tourToLocation;
            set
            {
                _tourToLocation = value;
                RaisePropertyChangedEvent(nameof(TourToLocation));
                CalculateDistanceCommand?.OnCanExecuteChanged();
                AddTourCommand?.OnCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                RaisePropertyChangedEvent(nameof(StatusMessage));
            }
        }

        public string TourDistanceInKm => _tourDistanceInKm.ToString() + " km";
        public ICommand NavigateHomeCommand { get; }
        public AsyncRelayCommand AddTourCommand { get; }

        public AsyncRelayCommand CalculateDistanceCommand { get; }

        public AddTourViewModel(ITourPlannerManager tourPlannerManager, NavigationStore navigationStore)
        {
            _tourPlannerManager = tourPlannerManager;
            _navigationStore = navigationStore;
            _tourName = String.Empty;
            _tourDescription = String.Empty;
            _tourFromLocation = String.Empty;
            _tourToLocation = String.Empty;
            _statusMessage = String.Empty;
            _tourDistanceInKm = 0;
            NavigateHomeCommand = new RelayCommand(o =>
            {
                navigationStore.CurrentViewModel = new HomeViewModel(_tourPlannerManager, navigationStore);
            });
            AddTourCommand = new AsyncRelayCommand(AddTourCommandMethod, CanExecuteAddTour, AddTourOnException);

            // AsyncCommand = new AsyncAddTourCommand(this, ex => { UtilMethods.ShowErrorMsgBox("Error"); });
            CalculateDistanceCommand = new AsyncRelayCommand(CalculateDistanceCommandMethod,
                CanExecuteCalculateDistance, CalculateDistanceException);
        }

        private async Task AddTourCommandMethod()
        {
            AsyncCommandIsExecuting = true;

            Tour t = await _tourPlannerManager.CreateTour(TourName, TourDescription, TourFromLocation,
                TourToLocation);
            if (t != null)
            {
                _navigationStore.CurrentViewModel = new HomeViewModel(_tourPlannerManager, _navigationStore);
            }

            if (t == null)
            {
                StatusMessage = "Error Creating a new Tour make sure the locations are valid!";
            }

            AsyncCommandIsExecuting = false;
        }

        private void CalculateDistanceException(Exception ex)
        {
            AsyncCommandIsExecuting = false;
            StatusMessage = $"Error calculating distance!{Environment.NewLine}{ex.Message}";
        }

        private void AddTourOnException(Exception ex)
        {
            AsyncCommandIsExecuting = false;
            StatusMessage = $"Error adding a new Tour!{Environment.NewLine}{ex.Message}";
        }

        private async Task CalculateDistanceCommandMethod()
        {
            AsyncCommandIsExecuting = true;
            double distance = await _tourPlannerManager.GetTourDistance(TourFromLocation, TourToLocation);
            if (distance > 0)
            {
                _tourDistanceInKm = distance;
                RaisePropertyChangedEvent(nameof(TourDistanceInKm));
            }
            if (distance <= 0)
            {
                StatusMessage = "Error Calculating Distance! Make sure locations are valid.";
            }

            AsyncCommandIsExecuting = false;
        }

        public bool CanExecuteCalculateDistance(object param)
        {
            return !String.IsNullOrWhiteSpace(TourFromLocation) &&
                   !String.IsNullOrWhiteSpace(TourToLocation);
        }


        public bool CanExecuteAddTour(object obj)
        {
            return !String.IsNullOrWhiteSpace(TourName) &&
                   !String.IsNullOrWhiteSpace(TourDescription) &&
                   !String.IsNullOrWhiteSpace(TourFromLocation) &&
                   !String.IsNullOrWhiteSpace(TourToLocation);
        }
    }
}