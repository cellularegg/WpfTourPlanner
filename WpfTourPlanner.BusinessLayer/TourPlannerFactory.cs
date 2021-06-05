using System;
using System.Configuration;
using WpfTourPlanner.Models.Exceptions;

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
                string mapQuestEnvironmentVarName = ConfigurationManager.AppSettings["MapQuestEnvironmentVarName"] ??
                                                    throw new ConfigException(
                                                        "MapQuestEnvironmentVarName is not provided in App.config!");
                string mapQuestApiKey = Environment.GetEnvironmentVariable(mapQuestEnvironmentVarName) ??
                                        throw new ConfigException(
                                            $"Mapquest Api key environment variable not set! Name of " +
                                            $"environment variable: {mapQuestEnvironmentVarName}");
                string workingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
                if (exportFileName != null)
                {
                    _manager = new TourPlannerManagerImpl(mapQuestApiKey, exportFileName, summaryReportFileName,
                        workingDirectory);
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