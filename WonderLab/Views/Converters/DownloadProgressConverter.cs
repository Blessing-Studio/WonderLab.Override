using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WonderLab.Views.Converters;

public class DownloadProgressConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        double progress = (double)value!;

        try {
            return $"{progress:0.00}%";
        }
        catch (Exception) {
        }

        return "0.00%";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}