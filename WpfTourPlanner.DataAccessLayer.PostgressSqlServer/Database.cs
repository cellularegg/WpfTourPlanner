using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Npgsql;
using WpfTourPlanner.DataAccessLayer.Common;

namespace WpfTourPlanner.DataAccessLayer.PostgressSqlServer
{
    public class Database: IDatabase
    {
        private string _connectionString;

        public Database(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DbCommand CreateCommand(string genericCommandText)
        {
            Debug.WriteLine($"Creating Command: {genericCommandText}");
            return new NpgsqlCommand(genericCommandText);
        }

        public int DeclareParameter(DbCommand command, string name, DbType type)
        {
            Debug.WriteLine($"Declaring parameter! Command: {command}, Name: {name}, Type: {type}");
            if (!command.Parameters.Contains(name))
            {
                int index = command.Parameters.Add(new NpgsqlParameter(name, type));
                return index;
            }

            throw new ArgumentException($"Parameter {name} already exists.");
        }

        public void DefineParameter(DbCommand command, string name, DbType type, object value)
        {
            Debug.WriteLine($"Defining parameter! Command: {command}, Name: {name}, Type: {type}, Value: {value}");

            int index = DeclareParameter(command, name, type);
            command.Parameters[index].Value = value;
        }

        public void SetParameter(DbCommand command, string name, object value)
        {
            Debug.WriteLine($"Setting parameter! Command: {command}, Name: {name}, Value: {value}");
            if (command.Parameters.Contains(name))
            {
                command.Parameters[name].Value = value;
                return;
            }

            // TODO unit test to make sure exception is thrown
            throw new ArgumentException($"Parameter {name} does not exists.");
        }

        public IDataReader ExecuteReader(DbCommand command)
        {
            DbConnection connection = CreateConnection();
            command.Connection = connection;
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public int ExecuteScalar(DbCommand command)
        {
            DbConnection connection = CreateConnection();
            command.Connection = connection;
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private DbConnection CreateConnection()
        {
            DbConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}