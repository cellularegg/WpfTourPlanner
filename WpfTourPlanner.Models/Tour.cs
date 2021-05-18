using System.Collections;
using System.Collections.Generic;

namespace WpfTourPlanner.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Information { get; set; }
        public double DistanceInKm { get; set; }

        public Tour(int id, string name, string description, string information, double distanceInKm)
        {
            Id = id;
            Name = name;
            Description = description;
            Information = information;
            DistanceInKm = distanceInKm;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description}, " +
                   $"{nameof(Information)}: {Information}, {nameof(DistanceInKm)}: {DistanceInKm}";
        }
    }
}