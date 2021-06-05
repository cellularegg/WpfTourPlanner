using System;

namespace WpfTourPlanner.Models.Exceptions
{
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base($"Config Error: {message}")
        {
        }
    }
}