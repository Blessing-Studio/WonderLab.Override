using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace WonderLab.UI.Converters;

public sealed class IconKindToTextAlignmentConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value is null
            ? TextAlignment.Left
            : TextAlignment.Center;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}