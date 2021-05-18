using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.ViewModels
{
    public class TourPlannerMainVm : ViewModelBase
    {
        private ITourPlannerManager _tourPlannerManager;
        private Tour _currentItem;
        private string _searchQuery;

        public ICommand SearchCommand { get;  }

        public ICommand ClearCommand { get;  }

        // TODO add other commands
        public ObservableCollection<Tour> Tours { get; set; }

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

        public Tour CurrentItem
        {
            get => _currentItem;
            set
            {
                if ((_currentItem != value) && (value != null))
                {
                    _currentItem = value;
                    RaisePropertyChangedEvent(nameof(CurrentItem));
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
            
            FillTourList();
        }

        private void FillTourList()
        {
            foreach (Tour tour in _tourPlannerManager.GetTours())
            {
                Tours.Add(tour);
            }
        }
    }
}