using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.BusinessLayer
{
    public class TourPlannerImpl : ITourPlanner

    {
        public IEnumerable<Tour> GetTours()
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            return tourDao.GetTours();
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
                temperatureInC, breaks, steps, logTour);
        }
        
        

    }
}