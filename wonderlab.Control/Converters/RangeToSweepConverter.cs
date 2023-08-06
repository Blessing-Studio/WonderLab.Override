using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control.Converters {
    public class RangeToSweepConverter : IMultiValueConverter {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
            double min = 0, max = 100, val = 0;

            for (var i = 0; i < values.Count; i++) {
                if (values[i] == null)
                    return null;
            }

            if (values[0] is double value)
                val = value;

            if (values[1] is double minimum)
                min = minimum;

            if (values[2] is double maximum)
                max = maximum;

            var m = max - min;
            return val / m * 360;
        }
    }
}
