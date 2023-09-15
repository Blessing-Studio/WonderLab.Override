using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace wonderlab.Views.Converters {
    public class DownloadProgressConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            var result = value as double?;

            if (result != null) {
                return $"{result!.Value:0.00}%";
            }

            return "11.4514%";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
