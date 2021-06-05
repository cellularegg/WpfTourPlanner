using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.BusinessLayer
{
    public interface ITourPlannerManager
    {
        IEnumerable<Tour> GetTours();

        IEnumerable<Tour> Search(string searchQuery);

        Tour CreateTour(string name, string description, string information, double distanceInKm);

        Tour DuplicateTour(Tour t);

        TourLog CreateTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps, Tour logTour);

        TourLog UpdateTourLog(int logId, string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps);

        Tour UpdateTour(int tourId, string name, string description, string information, double distanceInKm);

        bool DeleteTour(int tourId);

        bool DeleteTourLog(int tourLogId);

        bool Export(string folderPath);

        bool Import(string filePath);
        bool GenerateTourReport(Tour tour, string folderPath);
        bool GenerateSummaryReport(string folderPath);

        Task<double> GetTourDistance(string fromLocation, string toLocation);
        Task<Tour> CreateTour(string name, string description, string fromLocation, string toLocation);
    }
}