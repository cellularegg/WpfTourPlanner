using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
        private readonly string _exportFileName;

        private readonly string _summaryReportFileName;

        public TourPlannerManagerImpl(string exportFileName = "WpfTourPlanner.Json",
            string summaryReportFileName = "Report.pdf")
        {
            _exportFileName = exportFileName;
            _summaryReportFileName = summaryReportFileName;
        }

        public virtual IEnumerable<Tour> GetTours()
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            // ToDo check if this is ok
            IEnumerable<Tour> tours = tourDao.GetTours();
            foreach (Tour tour in tours.ToArray())
            {
                tour.Logs = new ObservableCollection<TourLog>(tour.Logs as List<TourLog> ?? new List<TourLog>());
            }

            return tours;
        }

        public IEnumerable<Tour> Search(string searchQuery)
        {
            IEnumerable<Tour> tours = GetTours();
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
            return tourDao.DuplicateTour(t);
        }

        public TourLog CreateTourLog(string report, DateTime logDateTime, double totalTimeInH, int rating,
            double heartRate, double averageSpeedInKmH, double temperatureInC, int breaks, int steps, Tour logTour)
        {
            ITourLogDao tourLogDao = DalFactory.CreateTourLogDao();
            return tourLogDao.AddNewTourLog(report, logDateTime, totalTimeInH, rating, heartRate, averageSpeedInKmH,
                temperatureInC, breaks, steps, logTour.Id);
        }

        public Tour UpdateTour(int tourId, string name, string description, string information, double distanceInKm)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
            return tourDao.UpdateTour(tourId, name, description, information, distanceInKm);
        }

        public bool DeleteTour(int tourId)
        {
            ITourDao tourDao = DalFactory.CreateTourDao();
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
                throw new InvalidImportFileException($"Error the file ({filePath}) does not exist!");
            }

            string fileContent = File.ReadAllText(filePath);

            try
            {
                List<Tour> importedTours = JsonConvert.DeserializeObject<List<Tour>>(fileContent);
                if (importedTours == null)
                {
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
                Debug.WriteLine(e);
                throw new InvalidImportFileException(
                    $"Error the file ({filePath}) is not a valid json file!{Environment.NewLine}{e.Message}");
            }

            return false;
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
            // TODO Try catch
            byte[] imageData = new byte[] { };
            if (File.Exists(tour.Information))
            {
                imageData = File.ReadAllBytes(tour.Information);
            }

            var document = new TourReportDocument(tour, imageData);
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
            // var model = InvoiceDocumentDataSource.GetInvoiceDetails();
            // var document = new InvoiceDocument(model);
            // TODO Try catch
            IEnumerable<Tour> tours = GetTours();
            var document = new SummaryReportDocument(tours);
            document.GeneratePdf(filePath);

            return true;
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
                    Directory.CreateDirectory(folderPath);
                }
                catch (DirectoryNotFoundException e)
                {
                    Debug.WriteLine("The specified path is invalid (for example, it is on an unmapped drive).");
                    Debug.WriteLine(e);
                    return false;
                }
                catch (NotSupportedException e)
                {
                    Debug.WriteLine(
                        "path contains a colon character (:) that is not part of a drive label (\"C:\\\").");
                    Debug.WriteLine(e);
                    return false;
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine("The caller does not have the required permission.");
                    Debug.WriteLine(e);
                    return false;
                }
                catch (PathTooLongException e)
                {
                    Debug.WriteLine(
                        "The specified path, file name, or both exceed the system-defined maximum length..");
                    Debug.WriteLine(e);
                    return false;
                }
                catch (IOException e)
                {
                    Debug.WriteLine("The directory specified by path is a file.The network name is not known.");
                    Debug.WriteLine(e);
                    return false;
                }
            }

            return true;
        }

    }
}