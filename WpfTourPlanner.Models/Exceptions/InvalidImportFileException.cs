using System;

namespace WpfTourPlanner.Models.Exceptions
{
    public class InvalidImportFileException : Exception
    {
        public InvalidImportFileException(string message) : base(message)
        {
        }
    }
}