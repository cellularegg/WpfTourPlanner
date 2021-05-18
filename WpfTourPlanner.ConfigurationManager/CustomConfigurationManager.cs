using System.Diagnostics;
using System.Windows.Threading;

namespace WpfTourPlanner.ConfigurationManager
{
    public class CustomConfigurationManager
    {
        // Signleton: https://csharpindepth.com/Articles/Singleton
        private static readonly CustomConfigurationManager _instance = new CustomConfigurationManager();

        public string ConnectionString { get; }

        public string AssemblyName { get; }

        // TODO add Logging configuration!
        static CustomConfigurationManager()
        {
        }

        private CustomConfigurationManager()
        {
            // TODO check if config file exists
            // TODO if not create a sample file
            // TODO if it exists read file

            AssemblyName = "WpfTourPlanner.DataAccessLayer.PostgressSqlServer";
            ConnectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;" +
                               "Password=mysecretpassword;";
        }

        public static CustomConfigurationManager Instance
        {
            get { return _instance; }
        }
    }
}