using Avalonia.Data.Converters;
using MinecraftLaunch.Modules.Models.Launch;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Views.Converters {
    public class GameCoreSubTitleConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            var core = (GameCore)value!;

            if(core == null) 
                return "Unknown";

            StringBuilder sb = new();
            if (core.HasModLoader) {
                sb.Append($"{core.InheritsFrom ?? core.Source} 依赖的加载器：")
                    .Append(string.Join('，'
                    ,core.ModLoaderInfos.Select(x => $"{x.ModLoaderType} {x.Version}")));

                return sb.ToString();
            }

            sb.Append($"原版 {core.Source ?? core.Id}");
            return sb.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
