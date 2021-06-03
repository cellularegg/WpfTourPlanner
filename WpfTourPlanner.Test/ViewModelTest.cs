using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models;
using WpfTourPlanner.Stores;
using WpfTourPlanner.ViewModels;

namespace WpfTourPlanner.Test
{
    public class ViewModelTest
    {
        // ToDo mock database / tourplannermanager to return static list of tours + logs
        // ToDo test setters of TourName, TourDescription and TourDistance
        private Mock<ITourPlannerManager> _tourPlannerManager;
        private IList<Tour> _toursMock;
        private NavigationStore _navigationStore;

        [SetUp]
        public void SetUp()
        {
            _tourPlannerManager = new Mock<ITourPlannerManager>();
            _toursMock = new List<Tour>
            {
                new Tour(0, "Test1", "Descr1", "Filepath", 10, new List<TourLog>()),
                new Tour(1, "Test2", "Descr2", "Filepath", 12, new List<TourLog>
                {
                    new TourLog(0, "REPORT", DateTime.Now, 1.5, 97.2, 12.5, 25.4, 2, 200, 3, 1)
                })
            };
            _tourPlannerManager.Setup(tpm => tpm.GetTours()).Returns(_toursMock);

            _navigationStore = new NavigationStore();
            _navigationStore.CurrentViewModel = null;
        }


        [Test]
        public void Test_Can_Execute_DuplicateTour()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.IsFalse(viewModel.CanExecuteDuplicateTourLog(null));
            viewModel.CurrentTour = _toursMock.FirstOrDefault();
            Assert.IsTrue(viewModel.CanExecuteDuplicateTour(null));
        }

        [Test]
        public void Test_Can_Execute_DeleteTour()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.IsFalse(viewModel.CanExecuteDeleteTour(null));
            viewModel.CurrentTour = _toursMock.FirstOrDefault();
            Assert.IsTrue(viewModel.CanExecuteDeleteTour(null));
        }

        [Test]
        public void Test_Can_Execute_GenerateTourReport()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.IsFalse(viewModel.CanExecuteGenerateTourReport(null));
            viewModel.CurrentTour = _toursMock.FirstOrDefault();
            Assert.IsTrue(viewModel.CanExecuteGenerateTourReport(null));
        }

        [Test]
        public void Test_Can_Execute_ClearSearch()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            viewModel.SearchQuery = string.Empty;
            Assert.IsFalse(viewModel.CanExecuteClearSearch(null));
            viewModel.SearchQuery = "String";
            Assert.IsTrue(viewModel.CanExecuteClearSearch(null));
        }

        [Test]
        public void Test_Can_Execute_DuplicateTourLog()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.IsFalse(viewModel.CanExecuteDuplicateTourLog(null));
            viewModel.CurrentTour = _toursMock[1];
            Assert.IsFalse(viewModel.CanExecuteDuplicateTourLog(null));
            viewModel.CurrentLog = viewModel.CurrentTour.Logs.FirstOrDefault();
            Assert.IsTrue(viewModel.CanExecuteDuplicateTourLog(null));
        }

        [Test]
        public void Test_Can_Execute_DeleteTourLog()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.IsFalse(viewModel.CanExecuteDeleteTourLog(null));
            viewModel.CurrentTour = _toursMock[1];
            Assert.IsFalse(viewModel.CanExecuteDeleteTourLog(null));
            viewModel.CurrentLog = viewModel.CurrentTour.Logs.FirstOrDefault();
            Assert.IsTrue(viewModel.CanExecuteDeleteTourLog(null));
        }

        [Test]
        public void Test_Can_Execute_UpdateTour()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.IsFalse(viewModel.CanExecuteUpdateTour(null));
            viewModel.CurrentTour = _toursMock.FirstOrDefault();
            Assert.IsTrue(viewModel.CanExecuteUpdateTour(null));
            viewModel.TourDescription = string.Empty;
            Assert.IsFalse(viewModel.CanExecuteUpdateTour(null));
            viewModel.TourDescription = "desc";
            viewModel.TourDistance = "1111a";
            Assert.IsFalse(viewModel.CanExecuteUpdateTour(null));
            viewModel.TourDistance = "-10";
            Assert.IsFalse(viewModel.CanExecuteUpdateTour(null));
        }

        [Test]
        public void Test_Get_TourDescription()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.AreEqual(string.Empty, viewModel.TourDescription);
            Tour t = _toursMock.FirstOrDefault();
            viewModel.CurrentTour = t;
            Assert.AreEqual(t.Description, viewModel.TourDescription);
        }

        [Test]
        public void Test_Get_TourName()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.AreEqual(string.Empty, viewModel.TourName);
            Tour t = _toursMock.FirstOrDefault();
            viewModel.CurrentTour = t;
            Assert.AreEqual(t.Name, viewModel.TourName);
        }

        [Test]
        public void Test_Get_TourDistance()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Assert.AreEqual(string.Empty, viewModel.TourDistance);
            Tour t = _toursMock.FirstOrDefault();
            viewModel.CurrentTour = t;
            Assert.AreEqual(t.DistanceInKm.ToString(), viewModel.TourDistance);
        }

        [Test]
        public void Test_Set_CurrentTour()
        {
            HomeViewModel viewModel = new HomeViewModel(_tourPlannerManager.Object, _navigationStore);
            Tour t = _toursMock.FirstOrDefault();
            viewModel.CurrentTour = t;
            Assert.AreEqual(t.Name, viewModel.TourName);
            Assert.AreEqual(t.Description, viewModel.TourDescription);
            Assert.AreEqual(t.DistanceInKm.ToString(), viewModel.TourDistance);
        }
    }
}