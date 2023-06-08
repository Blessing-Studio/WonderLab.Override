using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;

namespace wonderlab.Views.Converters {
    public class ColorUtilsToColorConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type? sourceType) {
            return sourceType == typeof(Color);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) {
            return destinationType == typeof(Color);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value) {
            if (value is Color c) {
                return ColorUtils.FromUInt(c.ToUint32());
            }
            return base.ConvertFrom(context, culture, value!)!;
        }

        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {
            if (value is ColorUtils c) {
                c.GetRGB(out byte r, out byte g, out byte b, out byte a);
                return new Color(a, r, g, b);
            }
            return base.ConvertTo(context, culture, value, destinationType)!;
        }
    }
}
