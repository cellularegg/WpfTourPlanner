using System;
using System.Security.Cryptography;

namespace WpfTourPlanner.Models
{
    public class TourLog
    {
        public const int RATING_MIN = 0;
        public const int RATING_MAX = 10;
        public int Id { get; set; }
        public string Report { get; set; }
        public DateTime LogDateTime { get; set; }

        public double TotalTimeInH { get; set; }

        // Rating from 0 - 10
        private int _rating;

        public int Rating
        {
            get => _rating;
            set
            {
                if (value < RATING_MIN)
                {
                    _rating = RATING_MIN;
                }
                else if (value > RATING_MAX)
                {
                    _rating = RATING_MAX;
                }
                else
                {
                    _rating = value;
                }
            }
        }

        public double HeartRate { get; set; }
        public double AverageSpeedInKmH { get; set; }
        public double TemperatureInC { get; set; }
        public int Breaks { get; set; }
        public int Steps { get; set; }
        public Tour LogTour { get; set; }

        public TourLog(int id, string report, DateTime logDateTime, double totalTimeInH,double heartRate,
            double averageSpeedInKmH, double temperatureInC, int breaks, int steps, int rating, Tour logTour)
        {
            Id = id;
            Report = report;
            LogDateTime = logDateTime;
            TotalTimeInH = totalTimeInH;
            HeartRate = heartRate;
            AverageSpeedInKmH = averageSpeedInKmH;
            TemperatureInC = temperatureInC;
            Breaks = breaks;
            Steps = steps;
            Rating = rating;
            LogTour = logTour;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Report)}: {Report}, {nameof(LogDateTime)}: {LogDateTime}, " +
                   $"{nameof(TotalTimeInH)}: {TotalTimeInH}, {nameof(Rating)}: {Rating}, {nameof(LogTour)}: " +
                   $"{LogTour}, {nameof(HeartRate)}: {HeartRate}, {nameof(AverageSpeedInKmH)}: {AverageSpeedInKmH}, " +
                   $"{nameof(TemperatureInC)}: {TemperatureInC}, {nameof(Breaks)}: {Breaks}, {nameof(Steps)}: {Steps}";
        }
    }
}