using Avalonia.Data.Converters;
using Avalonia.Media.Transformation;
using System;
using System.Globalization;

namespace WonderLab.Views.Converters;

public class ControlCenterTransformConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var opacity = (double)value!;
        return opacity is 0 ? TransformOperations.Parse("translate(0px)") : TransformOperations.Parse("translate(800px)");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}