using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.DataAccessLayer.PostgressSqlServer
{
    public class TourLogPostgressDao : ITourLogDao
    {
        private const string SQL_FIND_BY_ID = "SELECT * FROM public.\"TourLog\" WHERE \"Id\"=@Id;";
        private const string SQL_FIND_BY_MEDIA_ITEM = "SELECT * FROM public.\"TourLog\" WHERE \"TourId\"=@TourId;";

        private const string SQL_INSERT_NEW_TOURLOG = "INSERT INTO public.\"TourLog\" (\"Report\", \"LogDateTime\", " +
                                                      "\"TotalTimeInH\", \"Rating\", \"HeartRate\", " +
                                                      "\"AverageSpeedInKmH\", \"TemperatureInC\", \"Breaks\", " +
                                                      "\"Steps\", \"TourId\") VALUES (@Report, @LogDateTime, " +
                                                      "@TotalTimeInH, @Rating, @HeartRate, @AverageSpeedInKmH," +
                                                      " @TemperatureInC, @Breaks, @Steps, @TourId) RETURNING \"Id\";";

        private IDatabase _database;
        private ITourDao _tourDao;

        public TourLogPostgressDao()
        {
            _database = DalFactory.GetDatabase();
            _tourDao = DalFactory.CreateTourDao();
        }

        public TourLogPostgressDao(IDatabase database, ITourDao tourDao)
        {
            _database = database;
            _tourDao = tourDao;
        }

        public TourLog AddNewTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps, Tour logTour)
        {
            DbCommand insertCommand = _database.CreateCommand(SQL_INSERT_NEW_TOURLOG);
            _database.DefineParameter(insertCommand, "@Report", DbType.String, report);
            _database.DefineParameter(insertCommand, "@LogDateTime", DbType.String, logDateTime.ToString());
            _database.DefineParameter(insertCommand, "@TotalTimeInH", DbType.Double, totalTimeInH);
            _database.DefineParameter(insertCommand, "@Rating", DbType.Int32, rating);
            _database.DefineParameter(insertCommand, "@HeartRate", DbType.Double, heartRate);
            _database.DefineParameter(insertCommand, "@AverageSpeedInKmH", DbType.Double, averageSpeedInKmH);
            _database.DefineParameter(insertCommand, "@TemperatureInC", DbType.Double, temperatureInC);
            _database.DefineParameter(insertCommand, "@Breaks", DbType.Int32, breaks);
            _database.DefineParameter(insertCommand, "@Steps", DbType.Int32, steps);
            _database.DefineParameter(insertCommand, "@TourId", DbType.Int32, logTour.Id);
            return FindById(_database.ExecuteScalar(insertCommand));
        }

        public TourLog FindById(int logId)
        {
            DbCommand findByIdCommand = _database.CreateCommand(SQL_FIND_BY_ID);
            _database.DefineParameter(findByIdCommand, "@Id", DbType.Int32, logId);
            IEnumerable<TourLog> tourLogs = QueryTourLogsFromDb(findByIdCommand);
            return tourLogs.FirstOrDefault();
        }


        public IEnumerable<TourLog> GetLogsForTour(Tour tour)
        {
            DbCommand findByTourIdCommand = _database.CreateCommand(SQL_FIND_BY_ID);
            _database.DefineParameter(findByTourIdCommand, "@tourId", DbType.Int32, tour.Id);
            IEnumerable<TourLog> tourLogs = QueryTourLogsFromDb(findByTourIdCommand);
            return tourLogs;
        }

        private IEnumerable<TourLog> QueryTourLogsFromDb(DbCommand command)
        {
            List<TourLog> tourLogs = new List<TourLog>();
            using (IDataReader reader = _database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    tourLogs.Add(new TourLog(
                        (int) reader["Id"],
                        (string) reader["Report"],
                        DateTime.Parse(reader["LogDateTime"].ToString()),
                        (double) reader["TotalTimeInH"],
                        (double) reader["HeartRate"],
                        (double) reader["AverageSpeedInKmH"],
                        (double) reader["TemperatureInC"],
                        (int) reader["Breaks"],
                        (int) reader["Steps"],
                        (int) reader["Rating"],
                        _tourDao.FindById((int) reader["TourId"])
                    ));
                }
            }

            return tourLogs;
        }
    }
}