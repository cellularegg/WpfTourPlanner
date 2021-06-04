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
                string summaryReportFileName = ConfigurationManager.AppSettings["SummaryReportFileName"];

                if (exportFileName != null)
                {
                    _manager = new TourPlannerManagerImpl(exportFileName, summaryReportFileName);
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