using System.Collections.Generic;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.DataAccessLayer.Dao
{
    public interface ITourDao
    {
        Tour FindById(int tourId);
        Tour UpdateTour(int tourId, string name, string description, string information, double distanceInKm);
        Tour AddNewTour(string name, string description, string information, double distanceInKm);
        IEnumerable<Tour> GetTours();
        bool DeleteTour(int tourId);
    }
}