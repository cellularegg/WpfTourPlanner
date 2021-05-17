using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.DataAccessLayer.PostgressSqlServer
{
    public class TourPostgressDao : ITourDao
    {
        private const string SQL_FIND_BY_ID = "SELECT * FROM public.\"Tour\" WHERE \"Id\"=@Id;";
        private const string SQL_GET_ALL_ITEMS = "SELECT * FROM public.\"Tour\";";

        private const string SQL_INSERT_NEW_TOUR =
            "INSERT INTO \"Tour\" (\"Name\", \"Description\", \"Information\", \"DistanceInKm\") VALUES (@Name, @Description, @Information, @DistanceInKm) RETURNING \"Id\";";

        private IDatabase _database;

        public TourPostgressDao()
        {
            this._database = DalFactory.GetDatabase();
        }

        public TourPostgressDao(IDatabase database)
        {
            _database = database;
        }

        public Tour AddNewItem(string name, string description, string information, double distanceInKm)
        {
            DbCommand insertCommand = _database.CreateCommand(SQL_INSERT_NEW_TOUR);
            _database.DefineParameter(insertCommand, "@Name", DbType.String, name);
            _database.DefineParameter(insertCommand, "@Description", DbType.String, description);
            _database.DefineParameter(insertCommand, "@Information", DbType.String, information);
            _database.DefineParameter(insertCommand, "@DistanceInKm", DbType.Double, distanceInKm);
            return FindById(_database.ExecuteScalar(insertCommand));
        }


        public Tour FindById(int tourId)
        {
            DbCommand findCommand = _database.CreateCommand(SQL_FIND_BY_ID);
            _database.DefineParameter(findCommand, "@Id", DbType.Int32, tourId);
            IEnumerable<Tour> tours = QueryMediaItemsFromDatabase(findCommand);
            return tours.FirstOrDefault();
        }


        public IEnumerable<Tour> GetTours()
        {
            DbCommand getToursCommand = _database.CreateCommand(SQL_GET_ALL_ITEMS);
            return QueryMediaItemsFromDatabase(getToursCommand);
        }

        private IEnumerable<Tour> QueryMediaItemsFromDatabase(DbCommand command)
        {
            List<Tour> tourList = new List<Tour>();
            using (IDataReader reader = _database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    tourList.Add(new Tour(
                        (int) reader["Id"],
                        (string) reader["Name"],
                        (string) reader["Description"],
                        (string) reader["Information"],
                        (double) reader["DistanceInKm"]
                    ));
                }
            }

            return tourList;
        }
    }
}