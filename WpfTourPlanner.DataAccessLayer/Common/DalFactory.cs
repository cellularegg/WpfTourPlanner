using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using WpfTourPlanner.ConfigurationManager;
using WpfTourPlanner.DataAccessLayer.Dao;

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
            _assemblyName = CustomConfigurationManager.Instance.AssemblyName;
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
            // string connectionString =
            // ConfigurationManager.ConnectionStrings["PostgressSqlConnectionString"].ConnectionString;
            string connectionString = CustomConfigurationManager.Instance.ConnectionString;
            return CreateDatabase(connectionString);
        }

        private static IDatabase CreateDatabase(string connectionString)
        {
            string dataBaseClassName = _assemblyName + ".Database";
            Type dbClass = _dalAssembly.GetType(dataBaseClassName);
            return Activator.CreateInstance(dbClass, new object[] {connectionString}) as IDatabase;
        }

        public static ITourDao CreateTourDao()
        {
            string className = _assemblyName + ".TourPostgressDao";
            Type tourType = _dalAssembly.GetType(className);
            return Activator.CreateInstance(tourType) as ITourDao;
        }

        public static ITourLogDao CreateTourLogDao()
        {
            string className = _assemblyName + ".TourLogPostgressDao";
            Type tourLogType = _dalAssembly.GetType(className);
            return Activator.CreateInstance(tourLogType) as ITourLogDao; 
        }
    }
}