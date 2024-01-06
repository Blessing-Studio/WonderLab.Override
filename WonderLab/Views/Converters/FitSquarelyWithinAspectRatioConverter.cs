using System;
using Avalonia;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WonderLab.Views.Converters;

public class FitSquarelyWithinAspectRatioConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        Rect bounds = (Rect)value;
        return Math.Min(bounds.Width, bounds.Height);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}