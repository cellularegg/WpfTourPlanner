using System.Windows.Input;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models;
using WpfTourPlanner.Stores;

namespace WpfTourPlanner.ViewModels
{
    public class AddTourLogViewModel : ViewModelBase
    {
        private ITourPlannerManager _tourPlannerManager;

        public ICommand NavigateHomeCommand { get; }
        public Tour CurrentTour { get; }


        public AddTourLogViewModel(ITourPlannerManager tourPlannerManager, NavigationStore navigationStore, Tour currentTour)
        {
            _tourPlannerManager = tourPlannerManager;
            CurrentTour = currentTour;
            NavigateHomeCommand = new RelayCommand(o =>
            {
                navigationStore.CurrentViewModel = new HomeViewModel(_tourPlannerManager, navigationStore);
            });
        }
    }
}
