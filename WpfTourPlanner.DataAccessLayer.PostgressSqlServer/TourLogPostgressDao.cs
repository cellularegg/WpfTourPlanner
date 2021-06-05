using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Npgsql;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.DataAccessLayer.PostgressSqlServer
{
    public class TourLogPostgressDao : ITourLogDao
    {
        private const string SQL_FIND_BY_ID = "SELECT * FROM public.\"TourLog\" WHERE \"Id\"=@Id;";

        private const string SQL_FIND_BY_TOUR =
            "SELECT * FROM public.\"TourLog\" WHERE \"TourId\"=@TourId ORDER BY \"Id\";";

        private const string SQL_INSERT_NEW_TOURLOG = "INSERT INTO public.\"TourLog\" (\"Report\", \"LogDateTime\", " +
                                                      "\"TotalTimeInH\", \"Rating\", \"HeartRate\", " +
                                                      "\"AverageSpeedInKmH\", \"TemperatureInC\", \"Breaks\", " +
                                                      "\"Steps\", \"TourId\") VALUES (@Report, @LogDateTime, " +
                                                      "@TotalTimeInH, @Rating, @HeartRate, @AverageSpeedInKmH," +
                                                      " @TemperatureInC, @Breaks, @Steps, @TourId) RETURNING \"Id\";";

        private const string SQL_UPDATE_TOURLOG = "UPDATE public.\"TourLog\" SET \"Report\"=@Report, " +
                                                  "\"LogDateTime\"=@LogDateTime, \"TotalTimeInH\"=@TotalTimeInH, " +
                                                  "\"Rating\"=@Rating, \"HeartRate\"=@HeartRate, " +
                                                  "\"AverageSpeedInKmH\"=@AverageSpeedInKmH, " +
                                                  "\"TemperatureInC\"=@TemperatureInC, \"Breaks\"=@Breaks, " +
                                                  "\"Steps\"=@Steps WHERE \"Id\"=@Id RETURNING \"Id\";";

        private const string SQL_DELETE_TOUR_LOG = "DELETE FROM public.\"TourLog\" WHERE \"Id\"=@Id RETURNING \"Id\";";

        private IDatabase _database;

        public TourLogPostgressDao()
        {
            _database = DalFactory.GetDatabase();
        }

        public TourLogPostgressDao(IDatabase database)
        {
            _database = database;
        }

        public TourLog AddNewTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps, int tourId)
        {
            try
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
                _database.DefineParameter(insertCommand, "@TourId", DbType.Int32, tourId);
                return FindById(_database.ExecuteScalar(insertCommand));
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }

        public TourLog UpdateTourLog(int logId, string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps)
        {
            try
            {
                DbCommand updateCommand = _database.CreateCommand(SQL_UPDATE_TOURLOG);
                _database.DefineParameter(updateCommand, "@Id", DbType.Int32, logId);
                _database.DefineParameter(updateCommand, "@Report", DbType.String, report);
                _database.DefineParameter(updateCommand, "@LogDateTime", DbType.String, logDateTime.ToString());
                _database.DefineParameter(updateCommand, "@TotalTimeInH", DbType.Double, totalTimeInH);
                _database.DefineParameter(updateCommand, "@Rating", DbType.Int32, rating);
                _database.DefineParameter(updateCommand, "@HeartRate", DbType.Double, heartRate);
                _database.DefineParameter(updateCommand, "@AverageSpeedInKmH", DbType.Double, averageSpeedInKmH);
                _database.DefineParameter(updateCommand, "@TemperatureInC", DbType.Double, temperatureInC);
                _database.DefineParameter(updateCommand, "@Breaks", DbType.Int32, breaks);
                _database.DefineParameter(updateCommand, "@Steps", DbType.Int32, steps);
                return FindById(_database.ExecuteScalar(updateCommand));
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }

        public bool DeleteTourLog(int tourLogId)
        {
            try
            {
                DbCommand deleteCommand = _database.CreateCommand(SQL_DELETE_TOUR_LOG);
                _database.DefineParameter(deleteCommand, "@Id", DbType.Int32, tourLogId);
                return _database.ExecuteScalar(deleteCommand) == tourLogId;
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }


        public TourLog FindById(int logId)
        {
            DbCommand findByIdCommand = _database.CreateCommand(SQL_FIND_BY_ID);
            _database.DefineParameter(findByIdCommand, "@Id", DbType.Int32, logId);
            IEnumerable<TourLog> tourLogs = QueryTourLogsFromDb(findByIdCommand);
            return tourLogs.FirstOrDefault();
        }


        public IList<TourLog> GetLogsForTour(Tour tour)
        {
            return GetLogsByTourId(tour.Id);
        }

        public IList<TourLog> GetLogsByTourId(int tourId)
        {
            DbCommand findByTourIdCommand = _database.CreateCommand(SQL_FIND_BY_TOUR);
            _database.DefineParameter(findByTourIdCommand, "@TourId", DbType.Int32, tourId);
            IList<TourLog> tourLogs = QueryTourLogsFromDb(findByTourIdCommand);
            return tourLogs;
        }

        private IList<TourLog> QueryTourLogsFromDb(DbCommand command)
        {
            try
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
                            (int) reader["TourId"]
                        ));
                    }
                }

                return tourLogs;
            }
            catch (NpgsqlException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException($"Error with the database!{Environment.NewLine}{e.Message}");
            }
        }
    }
}