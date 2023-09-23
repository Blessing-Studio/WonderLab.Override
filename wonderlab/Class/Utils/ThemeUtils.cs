using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace wonderlab.Class.Utils {
    public static class ThemeUtils {
        public static void SetAccentColor(Color value) {
            Dispatcher.UIThread.Post(() => {
                ColorUtils utils = value;

                AccentColorResources["AccentColor"] = value;
                AccentColorResources["AccentColorLight1"] = (Color)utils.LightenPercent(0.15f);
                AccentColorResources["AccentColorLight2"] = (Color)utils.LightenPercent(0.30f);
                AccentColorResources["AccentColorLight3"] = (Color)utils.LightenPercent(0.45f);
                AccentColorResources["AccentColorDark1"] = (Color)utils.LightenPercent(-0.15f);
                AccentColorResources["AccentColorDark2"] = (Color)utils.LightenPercent(-0.30f);
                AccentColorResources["AccentColorDark3"] = (Color)utils.LightenPercent(-0.45f);
            });
        }

        public static Color GetColor(string name) {
            return (Color)AccentColorResources[name]!;
        }

        public static SolidColorBrush GetBrush(string name) {
            return (SolidColorBrush)AccentColorResources[name]!;
        }

        public static void Init() {
            AccentColorResources = (AvaloniaXamlLoader.Load(new Uri("avares://wonderlab.Control/Theme/BasicResource.axaml")) as ResourceDictionary)!;
            Application.Current!.Resources.MergedDictionaries.Add(AccentColorResources);
            SetAccentColor(Color.Parse("#F4D03F"));
        }

        public static ResourceDictionary AccentColorResources = new ResourceDictionary();
    }
}
