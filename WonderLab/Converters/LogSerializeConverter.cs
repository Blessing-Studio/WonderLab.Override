using System;
using System.Globalization;
using Avalonia.Data.Converters;
using WonderLab.Classes.Datas;

namespace WonderLab.Converters;

public sealed class LogSerializeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var data = value as LogData;
        if (data is null) {
            return null;
        }

        return $"[{data.Timestamp:HH:mm:ss}] [{data.Level.ToString().ToUpper()}] {data.Message}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}