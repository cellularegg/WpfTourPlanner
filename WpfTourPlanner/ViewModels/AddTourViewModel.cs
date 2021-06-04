using System.Windows.Input;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models;
using WpfTourPlanner.Stores;

namespace WpfTourPlanner.ViewModels
{
    public class AddTourViewModel : ViewModelBase
    {
        private ITourPlannerManager _tourPlannerManager;


        public ICommand NavigateHomeCommand { get; }

        public AddTourViewModel(ITourPlannerManager tourPlannerManager, NavigationStore navigationStore)
        {
            _tourPlannerManager = tourPlannerManager;
            NavigateHomeCommand = new RelayCommand(o =>
            {
                navigationStore.CurrentViewModel = new HomeViewModel(_tourPlannerManager, navigationStore);
            });
        }
    }
}
