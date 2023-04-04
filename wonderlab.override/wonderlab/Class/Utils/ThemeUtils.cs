using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class ThemeUtils {
        public static void SetAccentColor(Color value) {
            ColorUtils utils = value;

            AccentColorResources["AccentColor"] = value;
            AccentColorResources["AccentColorLight1"] = (Color)utils.LightenPercent(0.15f);
            AccentColorResources["AccentColorLight2"] = (Color)utils.LightenPercent(0.30f);
            AccentColorResources["AccentColorLight3"] = (Color)utils.LightenPercent(0.45f);
            AccentColorResources["AccentColorDark1"] = (Color)utils.LightenPercent(-0.15f);
            AccentColorResources["AccentColorDark2"] = (Color)utils.LightenPercent(-0.30f);
            AccentColorResources["AccentColorDark3"] = (Color)utils.LightenPercent(-0.45f);
        }

        public static void Init() {
            AccentColorResources = (AvaloniaXamlLoader.Load(new Uri("avares://wonderlab.control/Theme/BasicResource.axaml")) as ResourceDictionary)!;
            Application.Current!.Resources.MergedDictionaries.Add(AccentColorResources);

            SetAccentColor(Color.Parse("#F4D03F"));
        }

        public static ResourceDictionary AccentColorResources = new ResourceDictionary();
    }
}
