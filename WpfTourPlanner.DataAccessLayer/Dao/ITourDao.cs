using System.Collections.Generic;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.DataAccessLayer.Dao
{
    public interface ITourDao
    {
        Tour FindById(int tourId);
        Tour AddNewItem(string name, string description, string information, double distanceInKm);
        IEnumerable<Tour> GetTours();
        // TODO update and delete
    }
}