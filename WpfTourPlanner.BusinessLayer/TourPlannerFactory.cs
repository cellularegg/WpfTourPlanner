using System;
using System.Configuration;
using log4net;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.BusinessLayer
{
    public static class TourPlannerFactory
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
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
                Log.Info($"Creating a new tour planner manager with the following parameters: exportFileName=" +
                          $"{exportFileName}, summaryReportFileName={summaryReportFileName}, " +
                          $"mapQuestEnvironmentVarName={mapQuestEnvironmentVarName}, for mapQuestApiKey look in the " +
                          $"user environment variables.");
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
            
            Log.Info($"Returning {_manager}");

            return _manager;
        }
    }
}