using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control.Converters
{
    public class DownloadProgressConverter : IValueConverter {   
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            var result = value as double?;

            if (result != null) {
                return $"{Math.Round(result!.Value, 2)}%";
            }

            return "11.4514%";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {       
            throw new NotImplementedException();
        }
    }
}
