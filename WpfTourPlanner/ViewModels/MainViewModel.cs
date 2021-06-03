using System;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Stores;

namespace WpfTourPlanner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public MainViewModel(NavigationStore navigationStore)
        {
            //CurrentViewModel = new HomeViewModel(TourPlannerFactory.GetTourPlannerManager());
            //CurrentViewModel = new AddTourViewModel();
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            RaisePropertyChangedEvent(nameof(CurrentViewModel));
        }
    }
}
