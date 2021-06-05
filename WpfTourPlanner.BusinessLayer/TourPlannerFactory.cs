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
                string mapQuestApiKey = ConfigurationManager.AppSettings["MapQuestApiKey"];
                string workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
                if (exportFileName != null)
                {
                    _manager = new TourPlannerManagerImpl(mapQuestApiKey, exportFileName, summaryReportFileName, workingDirectory);
                }
                else
                {
                    _manager = new TourPlannerManagerImpl(mapQuestApiKey);
                }
            }

            return _manager;
        }
    }
}