using System;
using System.Data;
using System.Data.Common;
using NUnit.Framework;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.DataAccessLayer.Common;
using WpfTourPlanner.DataAccessLayer.Dao;
using WpfTourPlanner.DataAccessLayer.PostgressSqlServer;
using WpfTourPlanner.Models.Exceptions;

namespace WpfTourPlanner.Test
{
    public class PostgressSqlServerTest
    {
        [Test]
        public void Test_DeclareParameter()
        {
            // Arrange
            IDatabase db = new Database("connstr");
            string parameterName = "@Id";
            // Act
            DbCommand cmd = db.CreateCommand($"SELECT * FROM public.\"MediaItems\" WHERE \"Id\" ={parameterName}");
            db.DeclareParameter(cmd, parameterName, DbType.Int32);
            // Assert
            Assert.IsTrue(cmd.Parameters.Contains(parameterName));
            Assert.AreEqual(DbType.Int32, cmd.Parameters[parameterName].DbType);
            Assert.IsNull(cmd.Parameters[parameterName].Value);
        }

        [Test]
        public void Test_DeclareParameter_Duplicate_Name()
        {
            // Arrange
            IDatabase db = new Database("connstr");
            string parameterName = "@Id";
            // Act
            DbCommand cmd = db.CreateCommand($"SELECT * FROM public.\"MediaItems\" WHERE \"Id\" ={parameterName}");
            db.DeclareParameter(cmd, parameterName, DbType.Int32);
            // Assert
            var ex = Assert.Throws<ArgumentException>(() => db.DeclareParameter(cmd, parameterName, DbType.Int32));
            Assert.That(ex.Message, Is.EqualTo($"Parameter {parameterName} already exists."));
        }

        [Test]
        public void Test_SetParameter()
        {
            // Arrange
            IDatabase db = new Database("connstr");
            string parameterName = "@Id";
            int parameterValue = 123;
            // Act
            DbCommand cmd = db.CreateCommand($"SELECT * FROM public.\"MediaItems\" WHERE \"Id\" ={parameterName}");
            db.DeclareParameter(cmd, parameterName, DbType.Int32);
            // Assert
            Assert.IsTrue(cmd.Parameters.Contains(parameterName));
            Assert.AreEqual(DbType.Int32, cmd.Parameters[parameterName].DbType);
            Assert.IsNull(cmd.Parameters[parameterName].Value);

            db.SetParameter(cmd, parameterName, parameterValue);
            Assert.AreEqual(parameterValue, cmd.Parameters[parameterName].Value);
        }

        [Test]
        public void Test_SetParameter_Without_Declaring_Parameter()
        {
            // Arrange
            IDatabase db = new Database("connstr");
            string parameterName = "@Id";
            int parameterValue = 123;

            // Act
            DbCommand cmd = db.CreateCommand($"SELECT * FROM public.\"MediaItems\" WHERE \"Id\" ={parameterName}");

            var ex = Assert.Throws<ArgumentException>(() => db.SetParameter(cmd, parameterName, parameterValue));
            Assert.That(ex.Message, Is.EqualTo($"Parameter {parameterName} does not exists."));
        }

        [Test]
        public void Test_GetTours_Without_Database_Connection()
        {
            // Arrange
            IDatabase db = new Database("Server=localhost;Port=5432;Database=postgres;User Id=user;Password=123;");

            ITourDao td = new TourPostgressDao(db, new TourLogPostgressDao(db));
            // Act
            // Assert
            var ex = Assert.Throws<DatabaseException>(() => td.GetTours());
            Assert.That(ex.Message.Contains("Error with the database!"));
        }
    }
}