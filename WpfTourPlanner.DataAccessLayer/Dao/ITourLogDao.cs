using System;
using System.Collections.Generic;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.DataAccessLayer.Dao
{
    public interface ITourLogDao
    {
        TourLog FindById(int logId);

        TourLog AddNewTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating, double heartRate,
            double averageSpeedInKmH, double temperatureInC, int breaks, int steps, Tour logTour);

        IEnumerable<TourLog> GetLogsForTour(Tour tour);
        // TODO update and delete!
    }
}