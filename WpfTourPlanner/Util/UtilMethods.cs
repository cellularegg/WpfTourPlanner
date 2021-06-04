using System;
using System.Windows;

namespace WpfTourPlanner.Util
{
    public static class UtilMethods
    {
        public static bool IsValidNumber(string value, TypeCode typeCode, int lowerLimit = 0,
            int upperLimit = Int32.MaxValue)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            switch (typeCode)
            {
                case TypeCode.Int32:
                    if (Int32.TryParse(value, out int intVal))
                    {
                        if (intVal >= lowerLimit && intVal <= upperLimit)
                        {
                            return true;
                        }
                    }

                    return false;
                case TypeCode.Double:
                    if (Double.TryParse(value, out double doubleVal))
                    {
                        if (doubleVal >= lowerLimit && doubleVal <= upperLimit)
                        {
                            return true;
                        }
                    }

                    return false;
                default:
                    return false;
            }
        }
        public static void ShowErrorMsgBox(string content, string caption = "Error")
        {
            MessageBox.Show(content, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}