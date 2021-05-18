using System;
using System.Collections.Generic;
using NUnit.Framework;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.ConfigurationManager;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.Test
{
    public class OtherTests
    {
        [Test]
        public void TestConfigManager()
        {
            CustomConfigurationManager cm = CustomConfigurationManager.Instance;
            Console.WriteLine("test");
        }

        [Test]
        public void TestDb()
        {
            ITourPlannerManager manager = TourPlannerFactory.GetTourPlannerManager();
            IEnumerable<Tour> tours = manager.GetTours();
            Console.WriteLine("done");
        }
    }
}