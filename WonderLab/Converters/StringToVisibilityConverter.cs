using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WonderLab.Converters;

public sealed class StringToVisibilityConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}