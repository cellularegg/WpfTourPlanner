using System;
using System.Collections.Generic;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.DataAccessLayer.Dao
{
    public interface ITourLogDao
    {
        TourLog FindById(int logId);

        TourLog AddNewTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating, double heartRate,
            double averageSpeedInKmH, double temperatureInC, int breaks, int steps, int tourId);

        IEnumerable<TourLog> GetLogsForTour(Tour tour);
        IEnumerable<TourLog> GetLogsByTourId(int tourId);
        // TODO update and delete!
    }
}