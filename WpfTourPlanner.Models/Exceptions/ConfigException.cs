using System;

namespace WpfTourPlanner.Models.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}