using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Views.Converters {
    public class NavigationColorConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            string tag = value?.ToString()!;
            if(string.IsNullOrEmpty(tag)) return null;

            if (tag is "Home") {
                return Color.Parse("#187DF9");
            } else if(tag is "Download") {
                return Color.Parse("#187DF9");
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
