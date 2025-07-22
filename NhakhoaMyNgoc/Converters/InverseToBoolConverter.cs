using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NhakhoaMyNgoc.Converters
{
    public class InverseToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && int.TryParse(parameter?.ToString(), out int targetValue))
                return intValue == targetValue;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b && int.TryParse(parameter?.ToString(), out int targetValue))
                return targetValue;
            return Binding.DoNothing;
        }
    }
}
