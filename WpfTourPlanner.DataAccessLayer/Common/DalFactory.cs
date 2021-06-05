using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.DataAccessLayer.Common
{
    public class DalFactory
    {
        // TODO Write own config manager!
        private static string _assemblyName;
        private static Assembly _dalAssembly;
        private static IDatabase _database;

        static DalFactory()
        {
            // var path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            // Debug.WriteLine(path);
            _assemblyName = ConfigurationManager.AppSettings["DalSqlAssembly"] ??
                            throw new ConfigException("Error DalSqlAssembly not provided in App.config");
            // _assemblyName = "WpfTourPlanner.DataAccessLayer.PostgressSqlServer";
            // _assemblyName = CustomConfigurationManager.Instance.AssemblyName;
            Debug.WriteLine("---------------------------------------------------------------------------------");
            Debug.WriteLine(_assemblyName);
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
                throw new ConfigException("Error no connection string provided in App.config");
            }

            Debug.WriteLine("---------------------------------------------------------------------------------");
            Debug.WriteLine(connectionString);
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
                Debug.WriteLine(e);
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
                Debug.WriteLine(e);
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
                Debug.WriteLine(e);
                throw e.InnerException ?? e;
            }
        }
    }
}