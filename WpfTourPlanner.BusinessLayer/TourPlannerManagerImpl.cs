using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.BusinessLayer
{
    public class TourPlannerManagerImpl : ITourPlannerManager

    {
        public IEnumerable<Tour> GetTours()
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            // ToDo check if this is ok
            IEnumerable<Tour> tours = tourDao.GetTours();
            foreach (Tour tour in tours.ToArray())
            {
                tour.Logs = new ObservableCollection<TourLog>(tour.Logs as List<TourLog> ?? new List<TourLog>());
            }

            return tours;
        }

        public IEnumerable<Tour> Search(string searchQuery)
        {
            // TODO search tour logs as well
            IEnumerable<Tour> tours = GetTours();
            return tours.Where(t =>
                (t.Name.ToLower().Contains(searchQuery.ToLower())) ||
                (t.Description.ToLower().Contains(searchQuery.ToLower())));
        }

        public Tour CreateTour(string name, string description, string information, double distanceInKm)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            return tourDao.AddNewItem(name, description, information, distanceInKm);
        }

        public TourLog CreateTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps, Tour logTour)
        {
            ITourLogDao tourLogDao = DalFactory.CreateTourLogDao();
            return tourLogDao.AddNewTourLog(report, logDateTime, totalTimeInH, rating, heartRate, averageSpeedInKmH,
                temperatureInC, breaks, steps, logTour.Id);
        }

        public Tour UpdateTour(int tourId, string name, string description, string information, double distanceInKm)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            return tourDao.UpdateTour(tourId, name, description, information, distanceInKm);
        }
    }
}