using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using WpfTourPlanner.BusinessLayer.DocumentTemplates;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.BusinessLayer
{
    public class TourPlannerManagerImpl : ITourPlannerManager
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly string _exportFileName;

        private readonly string _summaryReportFileName;
        private readonly string _mapQuestApiKey;
        private readonly string _workingDirectory;

        public TourPlannerManagerImpl(string mapQuestApiKey, string exportFileName = "WpfTourPlanner.Json",
            string summaryReportFileName = "Report.pdf", string workingDirectory = null)
        {
            _exportFileName = exportFileName;
            _summaryReportFileName = summaryReportFileName;
            _mapQuestApiKey = mapQuestApiKey;
            if (workingDirectory == null)
            {
                _workingDirectory = Directory.GetCurrentDirectory();
            }
            else
            {
                _workingDirectory = workingDirectory;
                if (!Directory.Exists(_workingDirectory))
                {
                    Log.Info($"Directory: {workingDirectory} does not exists creating dirs.");
                    Directory.CreateDirectory(_workingDirectory);
                }
            }
        }

        public virtual IEnumerable<Tour> GetTours()
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            IEnumerable<Tour> tours = tourDao.GetTours();
            foreach (Tour tour in tours.ToArray())
            {
                tour.Logs = new ObservableCollection<TourLog>(tour.Logs as List<TourLog> ?? new List<TourLog>());
            }

            Log.Info($"Returning {tours?.Count()} tours.");
            return tours;
        }

        public IEnumerable<Tour> Search(string searchQuery)
        {
            IEnumerable<Tour> tours = GetTours();
            Log.Info($"Executing search with searchQuery={searchQuery}.");
            return tours.Where(t =>
                (t.Name.ToLower().Contains(searchQuery.ToLower())) ||
                (t.Description.ToLower().Contains(searchQuery.ToLower())) ||
                (t.Information.ToLower().Contains(searchQuery.ToLower())) ||
                t.Logs.FirstOrDefault(l => (l.Report.ToLower().Contains(searchQuery.ToLower())) ||
                                           (l.LogDateTime.ToString(CultureInfo.InvariantCulture).ToLower()
                                               .Contains(searchQuery.ToLower()))
                ) != null);
        }

        public Tour CreateTour(string name, string description, string information, double distanceInKm)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            return tourDao.AddNewTour(name, description, information, distanceInKm);
        }

        public Tour DuplicateTour(Tour t)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            ITourLogDao tourLogDao = DalFactory.CreateTourLogDao();
            string newFilePath = string.Empty;
            if (File.Exists(t.Information) && Path.GetDirectoryName(t.Information) != null)
            {
                newFilePath = Path.Combine(Path.GetDirectoryName(t.Information),
                    Path.GetFileNameWithoutExtension(t.Information) + "_copy" + Path.GetExtension(t.Information));
                File.Copy(t.Information, newFilePath, true);
                Log.Info($"Copying Tour image from {t.Information} to {newFilePath}");
            }

            Tour duplicate = tourDao.AddNewTour(t.Name + " Copy", t.Description, newFilePath, t.DistanceInKm);
            if (duplicate != null)
            {
                Log.Info($"Copied Tour copying {duplicate.Logs.Count} TourLogs.");
                foreach (TourLog log in t.Logs)
                {
                    tourLogDao.AddNewTourLog(log.Report + " Copy", log.LogDateTime, log.TotalTimeInH, log.Rating,
                        log.HeartRate, log.AverageSpeedInKmH, log.TemperatureInC, log.Breaks, log.Steps,
                        duplicate.Id);
                    duplicate.Logs.Add(log);
                }
            }

            return duplicate;
        }

        public TourLog CreateTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps, Tour logTour)
        {
            ITourLogDao tourLogDao = DalFactory.CreateTourLogDao();
            return tourLogDao.AddNewTourLog(report, logDateTime, totalTimeInH, rating, heartRate, averageSpeedInKmH,
                temperatureInC, breaks, steps, logTour.Id);
        }

        public TourLog UpdateTourLog(int logId, string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps)
        {
            ITourLogDao tourLogDao = DalFactory.CreateTourLogDao();
            return tourLogDao.UpdateTourLog(logId, report, logDateTime, totalTimeInH, rating, heartRate,
                averageSpeedInKmH, temperatureInC, breaks, steps);
        }

        public Tour UpdateTour(int tourId, string name, string description, string information, double distanceInKm)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            return tourDao.UpdateTour(tourId, name, description, information, distanceInKm);
        }

        public bool DeleteTour(int tourId)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            Tour tourToDelete = tourDao.FindById(tourId);
            if (tourToDelete == null)
            {
                return false;
            }

            // Check if tour image exists
            if (File.Exists(tourToDelete.Information))
            {
                try
                {
                    // Try to delete the image
                    File.Delete(tourToDelete.Information);
                }
                // 
                catch (UnauthorizedAccessException e)
                {
                    Log.Error($"User does not have access to {tourToDelete.Information}", e);
                }
                catch (IOException e)
                {
                    Log.Error($"IO Error when deleting {tourToDelete.Information}", e);
                }
            }

            return tourDao.DeleteTour(tourId);
        }

        public bool DeleteTourLog(int tourLogId)
        {
            ITourLogDao tourLogDao = DalFactory.CreateTourLogDao();
            return tourLogDao.DeleteTourLog(tourLogId);
        }

        public bool Export(string folderPath)
        {
            if (!CrateDirectory(folderPath))
            {
                return false;
            }

            IEnumerable<Tour> tours = this.GetTours();

            string jsonString = JsonConvert.SerializeObject(tours);

            string combinedPath = Path.Combine(folderPath, _exportFileName);
            File.WriteAllText(combinedPath, jsonString);

            return true;
        }


        public bool Import(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Log.Error($"Import file {filePath} does not exist!");
                throw new InvalidImportFileException($"Error the file ({filePath}) does not exist!");
            }

            string fileContent = File.ReadAllText(filePath);

            try
            {
                List<Tour> importedTours = JsonConvert.DeserializeObject<List<Tour>>(fileContent);
                if (importedTours == null)
                {
                    Log.Error($"The file {filePath} is not a valid json file.");
                    throw new InvalidImportFileException($"Error the file ({filePath}) is not a valid json file!");
                }

                foreach (Tour tour in importedTours)
                {
                    int tourId = CreateTour(tour.Name, tour.Description, tour.Information, tour.DistanceInKm).Id;
                    tour.Id = tourId;
                    if (tour.Logs != null)
                    {
                        foreach (TourLog log in tour.Logs)
                        {
                            int logId = CreateTourLog(log.Report, log.LogDateTime, log.TotalTimeInH, log.Rating,
                                log.HeartRate, log.AverageSpeedInKmH, log.TemperatureInC, log.Breaks, log.Steps,
                                tour).Id;
                            log.Id = logId;
                        }
                    }
                }
            }
            catch (JsonException e)
            {
                Log.Error($"The file {filePath} is not a valid json file.");
                throw new InvalidImportFileException(
                    $"Error the file ({filePath}) is not a valid json file!{Environment.NewLine}{e.Message}");
            }

            return true;
        }

        public bool GenerateTourReport(Tour tour, string folderPath)
        {
            string fileName = tour.Name + ".pdf";
            // Try to create the directory if it does not exist
            if (!CrateDirectory(folderPath))
            {
                return false;
            }

            string filePath = Path.Combine(folderPath, fileName);
            byte[] imageData = new byte[] { };
            if (File.Exists(tour.Information))
            {
                imageData = File.ReadAllBytes(tour.Information);
            }

            var document = new TourReportDocument(tour, imageData);
            Log.Info($"Saving tour report to {filePath}.");
            document.GeneratePdf(filePath);

            return true;
        }

        public bool GenerateSummaryReport(string folderPath)
        {
            // Try to create the directory if it does not exist
            if (!CrateDirectory(folderPath))
            {
                return false;
            }

            string filePath = Path.Combine(folderPath, _summaryReportFileName);
            IEnumerable<Tour> tours = GetTours();
            var document = new SummaryReportDocument(tours);
            Log.Info($"Saving tour summary report to {filePath}.");
            document.GeneratePdf(filePath);

            return true;
        }

        public async Task<double> GetTourDistance(string fromLocation, string toLocation)
        {
            using HttpClient client = new HttpClient();
            string requestUrl = $"https://www.mapquestapi.com/directions/v2/route?key={_mapQuestApiKey}&unit=km&" +
                                $"from={fromLocation}&to={toLocation}";
            Log.Info(
                $"Sending https request to \"https://www.mapquestapi.com/directions/v2/route?key=<PLACEHOLDER>&unit=km&" +
                $"from={fromLocation}&to={toLocation}\".");
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error("The http request was not successful! URL Requested: " +
                          "\"https://www.mapquestapi.com/directions/v2/route?key=<PLACEHOLDER>&unit=km&" +
                          $"from={fromLocation}&to={toLocation}\"");
                return -1;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject parsedResult = JObject.Parse(jsonResponse);
            double dist = parsedResult["route"]?["distance"]?.Value<double>() ?? -1;
            Log.Info($"Distance={dist}.");
            return dist;
        }

        public async Task<Tour> CreateTour(string name, string description, string fromLocation, string toLocation)
        {
            using HttpClient client = new HttpClient();
            // Directions API
            string requestUrl = $"https://www.mapquestapi.com/directions/v2/route?key={_mapQuestApiKey}&unit=km&" +
                                $"from={fromLocation}&to={toLocation}";
            Log.Info("Sending https request to: " +
                     "\"https://www.mapquestapi.com/directions/v2/route?key=<PLACEHOLDER>&unit=km&" +
                     $"from={fromLocation}&to={toLocation}\"");
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error("The http request was not successful! URL Requested: " +
                          "\"https://www.mapquestapi.com/directions/v2/route?key=<PLACEHOLDER>&unit=km&" +
                          $"from={fromLocation}&to={toLocation}\"");
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject parsedResult = JObject.Parse(jsonResponse);
            double? dist = parsedResult["route"]?["distance"]?.Value<double>();
            double? ulLat = parsedResult["route"]?["boundingBox"]?["ul"]?["lat"]?.Value<double>();
            double? ulLng = parsedResult["route"]?["boundingBox"]?["ul"]?["lng"]?.Value<double>();
            double? lrLat = parsedResult["route"]?["boundingBox"]?["lr"]?["lat"]?.Value<double>();
            double? lrLng = parsedResult["route"]?["boundingBox"]?["lr"]?["lng"]?.Value<double>();
            string sessionId = parsedResult["route"]?["sessionId"]?.Value<string>();
            if (dist == null || (dist is 0) || ulLat == null || ulLng == null || lrLat == null || lrLng == null ||
                sessionId == null)
            {
                return null;
            }

            string imgFormat = "jpg";
            requestUrl = $"https://www.mapquestapi.com/staticmap/v5/map?key={_mapQuestApiKey}&size=600,600&" +
                         $"session={sessionId}&boundingBox{ulLat},{ulLng},{lrLat},{lrLng}&format={imgFormat}";
            Log.Info("Sending https request to: " +
                     "\"https://www.mapquestapi.com/staticmap/v5/map?key=<PLACEHOLDER>&size=600,600&" +
                     $"session={sessionId}&boundingBox{ulLat},{ulLng},{lrLat},{lrLng}&format={imgFormat}");
            response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error("The http request was not successful! URL Requested: " +
                         "\"https://www.mapquestapi.com/staticmap/v5/map?key=<PLACEHOLDER>&size=600,600&" +
                         $"session={sessionId}&boundingBox{ulLat},{ulLng},{lrLat},{lrLng}&format={imgFormat}");
                return null;
            }

            byte[] byteResponse = await response.Content.ReadAsByteArrayAsync();
            string filePath = Path.Join(_workingDirectory, $"{Guid.NewGuid().ToString()}.{imgFormat}");
            await File.WriteAllBytesAsync(filePath, byteResponse);
            if (!File.Exists(filePath))
            {
                Log.Error($"Creation of the file: {filePath} failed!");
                return null;
            }

            return CreateTour((string) name, (string) description, (string) filePath, (double) dist);
        }

        private static bool CrateDirectory(string folderPath)
        {
            if (String.IsNullOrWhiteSpace(folderPath))
            {
                return false;
            }

            if (!Directory.Exists(folderPath) && !String.IsNullOrWhiteSpace(folderPath))
            {
                try
                {
                    Log.Info($"Trying to create the directory: {folderPath}.");
                    Directory.CreateDirectory(folderPath);
                }
                catch (DirectoryNotFoundException e)
                {
                    Log.Error(
                        $"The specified path ({folderPath}) is invalid (for example, it is on an unmapped drive).", e);
                    // Debug.WriteLine("The specified path is invalid (for example, it is on an unmapped drive).");
                    // Debug.WriteLine(e);
                    return false;
                }
                catch (NotSupportedException e)
                {
                    Log.Error($"path ({folderPath}) contains a colon character (:) that is not part of a " +
                              $"drive label (\"C:\\\").", e);
                    // Debug.WriteLine(
                    // "path contains a colon character (:) that is not part of a drive label (\"C:\\\").");
                    // Debug.WriteLine(e);
                    return false;
                }
                catch (UnauthorizedAccessException e)
                {
                    Log.Error($"The caller does not have the required permission to create the the folder: " +
                              $"{folderPath}.", e);
                    // Debug.WriteLine("The caller does not have the required permission.");
                    // Debug.WriteLine(e);
                    return false;
                }
                catch (PathTooLongException e)
                {
                    Log.Error($"The specified path ({folderPath}), file name, or both exceed the " +
                              $"system-defined maximum length..", e);
                    // Debug.WriteLine(
                    // "The specified path, file name, or both exceed the system-defined maximum length..");
                    // Debug.WriteLine(e);
                    return false;
                }
                catch (IOException e)
                {
                    Log.Error($"The path ({folderPath}) is either a file or the network name is not known", e);
                    // Debug.WriteLine("The directory specified by path is a file.The network name is not known.");
                    // Debug.WriteLine(e);
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return $"TourPlannerManagerImpl: {nameof(_exportFileName)}: {_exportFileName}, " +
                   $"{nameof(_summaryReportFileName)}: {_summaryReportFileName}, " +
                   $"{nameof(_workingDirectory)}: {_workingDirectory}";
        }
    }
}