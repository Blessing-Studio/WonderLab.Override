using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MinecraftLaunch.Modules.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control.Converters {
    public class ModLoaderImageConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            var type = (ModLoaderType)value!;

            try {
                Trace.WriteLine(type);
                return GetIconBitmap($"{type}.png");
            }
            catch (Exception) {
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public static IImage GetIconBitmap(string uri) {
            using (var s = AssetLoader.Open(new Uri($"avares://wonderlab.control/Icons/{uri}"))) {
                return new Bitmap(s);
            }

            throw new Exception("获取 Icon 失败，可能是不存在或类型不是 AvaloniaResource 导致的");
        }
    }
}
