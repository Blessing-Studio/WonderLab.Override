using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WonderLab.Views.Converters;

public class Int32ToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var intValue = System.Convert
            .ToInt32(value);

        return intValue is 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}