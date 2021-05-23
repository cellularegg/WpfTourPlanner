using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using NUnit.Framework;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.Test
{
    public class ModelsTest
    {
        [Test]
        public void TestTourLogRating()
        {
            // Äquivalenzklassentest
            Tour t = new Tour(1, "name", "description", "img path", 10, null);
            TourLog tl = new TourLog(1, "report", DateTime.Now, 10, 100, 10, 30, 0, 1000, 0, t.Id);
            tl.Rating = TourLog.RATING_MAX + 1;
            Assert.AreEqual(TourLog.RATING_MAX, tl.Rating);
            tl.Rating = TourLog.RATING_MAX;
            Assert.AreEqual(TourLog.RATING_MAX, tl.Rating);
            tl.Rating = TourLog.RATING_MAX - 1;
            Assert.AreEqual(TourLog.RATING_MAX - 1, tl.Rating);
            tl.Rating = (int) TourLog.RATING_MAX / 2;
            Assert.AreEqual((int) TourLog.RATING_MAX / 2, tl.Rating);
            tl.Rating = TourLog.RATING_MIN + 1;
            Assert.AreEqual(TourLog.RATING_MIN + 1, tl.Rating);
            tl.Rating = 0;
            Assert.AreEqual(0, tl.Rating);
            tl.Rating = TourLog.RATING_MIN - 1;
            Assert.AreEqual(TourLog.RATING_MIN, tl.Rating);
        }
    }
}