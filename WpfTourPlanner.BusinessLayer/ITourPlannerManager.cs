using System;
using System.Collections.Generic;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.BusinessLayer
{
    public interface ITourPlannerManager
    {
        IEnumerable<Tour> GetTours();

        IEnumerable<Tour> Search(string searchQuery);

        Tour CreateTour(string name, string description, string information, double distanceInKm);

        TourLog CreateTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps, Tour logTour);

        Tour UpdateTour(int tourId, string name, string description, string information, double distanceInKm);

        bool DeleteTour(int tourId);

        bool DeleteTourLog(int tourLogId);
        
        bool Export(string folderPath);
        
        bool Import(string filePath);
    }
}