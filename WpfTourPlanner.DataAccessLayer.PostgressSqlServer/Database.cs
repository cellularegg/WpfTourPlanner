using System;
using System.Data;
using System.Data.Common;
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
            return new NpgsqlCommand(genericCommandText);
        }

        public int DeclareParameter(DbCommand command, string name, DbType type)
        {
            // TODO Unit test to make sure exception is thrown
            if (!command.Parameters.Contains(name))
            {
                int index = command.Parameters.Add(new NpgsqlParameter(name, type));
                return index;
            }

            throw new ArgumentException($"Parameter {name} already exists.");
        }

        public void DefineParameter(DbCommand command, string name, DbType type, object value)
        {
            int index = DeclareParameter(command, name, type);
            command.Parameters[index].Value = value;
        }

        public void SetParameter(DbCommand command, string name, object value)
        {
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