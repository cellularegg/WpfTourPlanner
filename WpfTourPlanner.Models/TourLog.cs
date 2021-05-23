using System;
using Newtonsoft.Json;

namespace WpfTourPlanner.Models
{
    public class TourLog
    {
        public const int RATING_MIN = 0;
        public const int RATING_MAX = 10;
        [JsonIgnore] public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Report { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime LogDateTime { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double TotalTimeInH { get; set; }

        // Rating from 0 - 10
        private int _rating;

        [JsonProperty(Required = Required.Always)]
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

        [JsonProperty(Required = Required.Always)]
        public double HeartRate { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double AverageSpeedInKmH { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double TemperatureInC { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Breaks { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Steps { get; set; }

        [JsonIgnore] public int TourId { get; set; }

        public TourLog(int id, string report, DateTime logDateTime, double totalTimeInH, double heartRate,
            double averageSpeedInKmH, double temperatureInC, int breaks, int steps, int rating, int tourId)
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
            TourId = tourId;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Report)}: {Report}, {nameof(LogDateTime)}: {LogDateTime}, " +
                   $"{nameof(TotalTimeInH)}: {TotalTimeInH}, {nameof(Rating)}: {Rating}, {nameof(HeartRate)}: " +
                   $"{HeartRate}, {nameof(AverageSpeedInKmH)}: {AverageSpeedInKmH}, {nameof(TemperatureInC)}: " +
                   $"{TemperatureInC}, {nameof(Breaks)}: {Breaks}, {nameof(Steps)}: {Steps}, {nameof(TourId)}: {TourId}";
        }
    }
}