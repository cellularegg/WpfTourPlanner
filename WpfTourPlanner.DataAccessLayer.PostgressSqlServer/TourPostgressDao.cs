using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Npgsql;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.DataAccessLayer.PostgressSqlServer
{
    public class TourPostgressDao : ITourDao
    {
        private const string SQL_FIND_BY_ID = "SELECT * FROM public.\"Tour\" WHERE \"Id\"=@Id;";
        private const string SQL_GET_ALL_ITEMS = "SELECT * FROM public.\"Tour\" ORDER BY \"Id\";";

        private const string SQL_INSERT_NEW_TOUR =
            "INSERT INTO public.\"Tour\" (\"Name\", \"Description\", \"Information\", \"DistanceInKm\") VALUES (@Name, @Description, @Information, @DistanceInKm) RETURNING \"Id\";";

        private const string SQL_UPDATE_TOUR =
            "UPDATE public.\"Tour\" SET \"Name\"=@Name, \"Description\"=@Description, \"Information\"=@Information, \"DistanceInKm\"=@DistanceInKm WHERE \"Id\"=@Id RETURNING \"Id\";";

        private const string SQL_DELETE_TOUR = "DELETE FROM public.\"Tour\" WHERE \"Id\"=@Id RETURNING \"Id\";";

        private IDatabase _database;

        private ITourLogDao _tourLogDao;

        public TourPostgressDao()
        {
            this._database = DalFactory.GetDatabase();
            _tourLogDao = DalFactory.CreateTourLogDao();
        }

        public TourPostgressDao(IDatabase database, ITourLogDao tourLogDao)
        {
            _database = database;
            _tourLogDao = tourLogDao;
        }

        public Tour AddNewTour(string name, string description, string information, double distanceInKm)
        {
            try
            {
                DbCommand insertCommand = _database.CreateCommand(SQL_INSERT_NEW_TOUR);
                _database.DefineParameter(insertCommand, "@Name", DbType.String, name);
                _database.DefineParameter(insertCommand, "@Description", DbType.String, description);
                _database.DefineParameter(insertCommand, "@Information", DbType.String, information);
                _database.DefineParameter(insertCommand, "@DistanceInKm", DbType.Double, distanceInKm);
                return FindById(_database.ExecuteScalar(insertCommand));
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }

        public Tour UpdateTour(int tourId, string name, string description, string information, double distanceInKm)
        {
            try
            {
                DbCommand updateCommand = _database.CreateCommand(SQL_UPDATE_TOUR);
                _database.DefineParameter(updateCommand, "@Id", DbType.Int32, tourId);
                _database.DefineParameter(updateCommand, "@Name", DbType.String, name);
                _database.DefineParameter(updateCommand, "@Description", DbType.String, description);
                _database.DefineParameter(updateCommand, "@Information", DbType.String, information);
                _database.DefineParameter(updateCommand, "@DistanceInKm", DbType.Double, distanceInKm);
                return FindById(_database.ExecuteScalar(updateCommand));
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }

        public bool DeleteTour(int tourId)
        {
            Tour tourToDelete = this.FindById(tourId);
            if (tourToDelete == null)
            {
                return false;
            }

            // Check if tour image exists
            if (File.Exists(tourToDelete.Information))
            {
                try
                {
                    // Try to delete the image
                    File.Delete(tourToDelete.Information);
                }
                // 
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e);
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e);
                }
            }


            try
            {
                DbCommand deleteCommand = _database.CreateCommand(SQL_DELETE_TOUR);
                _database.DefineParameter(deleteCommand, "@Id", DbType.Int32, tourId);
                return _database.ExecuteScalar(deleteCommand) == tourId;
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }

        public Tour DuplicateTour(Tour tour)
        {
            string newFilePath = string.Empty;
            if (File.Exists(tour.Information) && Path.GetDirectoryName(tour.Information) != null)
            {
                newFilePath = Path.Combine(Path.GetDirectoryName(tour.Information),
                    Path.GetFileNameWithoutExtension(tour.Information) + "_copy" + Path.GetExtension(tour.Information));
                File.Copy(tour.Information, newFilePath, true);
            }

            Tour newlyCreatedTour = this.AddNewTour(tour.Name + " Copy", tour.Description, newFilePath, tour.DistanceInKm);

            if (newlyCreatedTour != null)
            {
                foreach (TourLog log in tour.Logs)
                {
                    _tourLogDao.AddNewTourLog(log.Report + " Copy", log.LogDateTime, log.TotalTimeInH, log.Rating,
                        log.HeartRate, log.AverageSpeedInKmH, log.TemperatureInC, log.Breaks, log.Steps,
                        newlyCreatedTour.Id);
                    newlyCreatedTour.Logs.Add(log);
                }
            }

            return newlyCreatedTour;
        }


        public Tour FindById(int tourId)
        {
            try
            {
                DbCommand findCommand = _database.CreateCommand(SQL_FIND_BY_ID);
                _database.DefineParameter(findCommand, "@Id", DbType.Int32, tourId);
                IEnumerable<Tour> tours = QueryMediaItemsFromDatabase(findCommand);
                return tours.FirstOrDefault();
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }


        public IEnumerable<Tour> GetTours()
        {
            try
            {
                DbCommand getToursCommand = _database.CreateCommand(SQL_GET_ALL_ITEMS);
                return QueryMediaItemsFromDatabase(getToursCommand);
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
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
                        (double) reader["DistanceInKm"],
                        _tourLogDao.GetLogsByTourId((int) reader["Id"])
                    ));
                }
            }

            return tourList;
        }
    }
}