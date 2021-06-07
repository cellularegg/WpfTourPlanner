using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using log4net;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.DataAccessLayer.Common
{
    public class DalFactory
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static string _assemblyName;
        private static Assembly _dalAssembly;
        private static IDatabase _database;

        static DalFactory()
        {
            _assemblyName = ConfigurationManager.AppSettings["DalSqlAssembly"];
            if (String.IsNullOrWhiteSpace(_assemblyName))
            {
                Log.Error("DalSqlAssembly key was not provided in App.config.");
                throw new ConfigException("Error DalSqlAssembly not provided in App.config");
            }
            _dalAssembly = Assembly.Load(_assemblyName);
        }

        public static IDatabase GetDatabase()
        {
            if (_database == null)
            {
                _database = CreateDatabase();
            }

            return _database;
        }

        private static IDatabase CreateDatabase()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["PostgressSqlConnectionString"]?.ConnectionString;
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                Log.Error("Error no connection string provided in App.config (with the key " +
                          "\"PostgressSqlConnectionString\").");
                throw new ConfigException("Error no connection string provided in App.config");
            }
            return CreateDatabase(connectionString);
        }

        private static IDatabase CreateDatabase(string connectionString)
        {
            string dataBaseClassName = _assemblyName + ".Database";
            Type dbClass = _dalAssembly.GetType(dataBaseClassName);
            try
            {
                return Activator.CreateInstance(dbClass, new object[] {connectionString}) as IDatabase;
            }
            catch (TargetInvocationException e)
            {
                // Debug.WriteLine(e);
                Log.Error($"Error when trying to create an instance of the Database using reflection " +
                          $"{nameof(dataBaseClassName)}={dataBaseClassName}");
                throw e.InnerException ?? e;
            }
        }

        public static ITourDao CreateTourDao()
        {
            string className = _assemblyName + ".TourPostgressDao";
            Type tourType = _dalAssembly.GetType(className);
            try
            {
                return Activator.CreateInstance(tourType) as ITourDao;
            }
            catch (TargetInvocationException e)
            {
                // Debug.WriteLine(e);
                Log.Error($"Error when trying to create an instance of the Database using reflection " +
                          $"{nameof(className)}={className}");
                throw e.InnerException ?? e;
            }
        }

        public static ITourLogDao CreateTourLogDao()
        {
            string className = _assemblyName + ".TourLogPostgressDao";
            Type tourLogType = _dalAssembly.GetType(className);
            try
            {
                return Activator.CreateInstance(tourLogType) as ITourLogDao;
            }
            catch (TargetInvocationException e)
            {
                // Debug.WriteLine(e);
                Log.Error($"Error when trying to create an instance of the Database using reflection " +
                          $"{nameof(className)}={className}");
                throw e.InnerException ?? e;
            }
        }
    }
}