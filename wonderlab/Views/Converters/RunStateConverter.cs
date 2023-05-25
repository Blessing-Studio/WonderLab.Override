using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Enum;

namespace wonderlab.Views.Converters
{
    public class RunStateConverter : IValueConverter {   
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            var result = (RunState)value!;

            if(result is RunState.Normal) {
                return "正常";
            }

            return "未响应";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return new object();
        }

    }
}
