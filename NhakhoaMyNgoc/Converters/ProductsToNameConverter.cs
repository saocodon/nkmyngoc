using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NhakhoaMyNgoc.Converters
{
    public class ProductsToNameConverter : IValueConverter
    {
        public List<Product> Products { get; set; } = [];

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id)
            {
                return Products.FirstOrDefault(s => s.Id == id)?.Name ?? "(Chưa chọn)";
            }
            return "(Không hợp lệ)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
