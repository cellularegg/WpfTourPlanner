using System.Configuration;

namespace WpfTourPlanner.BusinessLayer
{
    public static class TourPlannerFactory
    {
        private static ITourPlannerManager _manager;

        public static ITourPlannerManager GetTourPlannerManager()
        {
            if (_manager == null)
            {
                string exportFileName = ConfigurationManager.AppSettings["ExportFileName"];
                if (exportFileName != null)
                {
                    _manager = new TourPlannerManagerImpl(exportFileName);
                }
                else
                {
                    _manager = new TourPlannerManagerImpl();
                }
            }

            return _manager;
        }
    }
}