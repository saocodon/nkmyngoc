using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NhakhoaMyNgoc.Converters
{
    public class RevisitToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return false;

            var revisit = values[0] as DateTime?;
            var visit = values[1] as DateTime?;

            if (!revisit.HasValue || !visit.HasValue)
                return false;

            return visit.Value.AddYears(1) >= revisit;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
