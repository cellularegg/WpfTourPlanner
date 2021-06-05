using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.Test
{
    public class BusinessLayerTest
    {
        [Test]
        public void Test_Invalid_Import()
        {
            TourPlannerManagerImpl tp = new TourPlannerManagerImpl(String.Empty);
            string filePath = "nonExistent.json";
            var ex = Assert.Throws<InvalidImportFileException>(() => tp.Import(filePath));
            Assert.That(ex.Message, Is.EqualTo($"Error the file ({filePath}) does not exist!"));
        }

        [Test]
        public void Test_Search_For_Name()
        {
            var tpMock = new Mock<TourPlannerManagerImpl>("apikey","Export.json", "Report.pdf", ".");
            IList<Tour> toursMock = new List<Tour>
            {
                new Tour(0, "Name", "Descr1", "Filepath", 10, new List<TourLog>()),
                new Tour(1, "Test2", "Descr2", "Filepath", 12, new List<TourLog>
                {
                    new TourLog(0, "REPORT", DateTime.Now, 1.5, 97.2, 12.5, 25.4, 2, 200, 3, 1)
                })
            };
            tpMock.Setup(tpm => tpm.GetTours()).Returns(toursMock);
            string searchQuery = "Name";
            IEnumerable<Tour> searchResult = tpMock.Object.Search(searchQuery);
            List<Tour> searchList = new List<Tour>(searchResult);
            Assert.IsTrue(searchList.Contains(toursMock[0]));
            Assert.IsFalse(searchList.Contains(toursMock[1]));
        }
        [Test]
        public void Test_Search_For_Description()
        {
            var tpMock = new Mock<TourPlannerManagerImpl>("apikey","Export.json", "Report.pdf", ".");
            IList<Tour> toursMock = new List<Tour>
            {
                new Tour(0, "Name", "blablabla", "Filepath", 10, new List<TourLog>()),
                new Tour(1, "Test2", "Some Description", "Filepath", 12, new List<TourLog>
                {
                    new TourLog(0, "REPORT", DateTime.Now, 1.5, 97.2, 12.5, 25.4, 2, 200, 3, 1)
                })
            };
            tpMock.Setup(tpm => tpm.GetTours()).Returns(toursMock);
            string searchQuery = "description";
            IEnumerable<Tour> searchResult = tpMock.Object.Search(searchQuery);
            List<Tour> searchList = new List<Tour>(searchResult);
            Assert.IsTrue(searchList.Contains(toursMock[1]));
            Assert.IsFalse(searchList.Contains(toursMock[0]));
        }
        
        [Test]
        public void Test_Search_For_TourLog_Report()
        {
            var tpMock = new Mock<TourPlannerManagerImpl>("apikey","Export.json", "Report.pdf", ".");
            IList<Tour> toursMock = new List<Tour>
            {
                new Tour(0, "Name", "blablabla", "Filepath", 10, new List<TourLog>()),
                new Tour(1, "Test2", "Some Description", "Filepath", 12, new List<TourLog>
                {
                    new TourLog(0, "report content", DateTime.Now, 1.5, 97.2, 12.5, 25.4, 2, 200, 3, 1)
                })
            };
            tpMock.Setup(tpm => tpm.GetTours()).Returns(toursMock);
            string searchQuery = "report";
            IEnumerable<Tour> searchResult = tpMock.Object.Search(searchQuery);
            List<Tour> searchList = new List<Tour>(searchResult);
            Assert.IsTrue(searchList.Contains(toursMock[1]));
            Assert.IsFalse(searchList.Contains(toursMock[0]));
        }
    }
}