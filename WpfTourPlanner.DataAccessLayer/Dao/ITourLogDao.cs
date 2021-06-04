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

        TourLog UpdateTourLog(int logId, string report, DateTime logDateTime, double totalTimeInH, int rating, double heartRate,
            double averageSpeedInKmH, double temperatureInC, int breaks, int steps);
        IList<TourLog> GetLogsForTour(Tour tour);
        IList<TourLog> GetLogsByTourId(int tourId);
        bool DeleteTourLog(int tourLogId);
    }
}