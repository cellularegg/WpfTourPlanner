using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace WpfTourPlanner.Models
{
    public class Tour
    {
        [JsonIgnore] public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Information { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double DistanceInKm { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public IList<TourLog> Logs { get; set; }

        public Tour(int id, string name, string description, string information, double distanceInKm,
            IList<TourLog> logs)
        {
            Id = id;
            Name = name;
            Description = description;
            Information = information;
            DistanceInKm = distanceInKm;
            Logs = logs;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description}, " +
                   $"{nameof(Information)}: {Information}, {nameof(DistanceInKm)}: {DistanceInKm}";
        }
    }
}