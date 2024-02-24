using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WonderLab.Converters;

public sealed class BooleanToIntegerConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return System.Convert.ToBoolean(value) ? 1 : 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}