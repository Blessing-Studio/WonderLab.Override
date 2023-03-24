using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control.Theme
{
    /// <summary>
    /// 颜色帮助器
    /// </summary>
    [Obsolete]
    public class ColorHelper
    {
        public void GetSystemAccentColor() {       

        }

        private void LoadCustomAccentColor()
        {
            //if (!_customAccentColor.HasValue)
            //{
            //    if (PreferUserAccentColor)
            //    {
            //        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //        {
            //            try
            //            {
            //                var settings = WinRTInterop.CreateInstance<IUISettings3>("Windows.UI.ViewManagement.UISettings");
            //                TryLoadWindowsAccentColor(settings);
            //            }
            //            catch
            //            {
            //                LoadDefaultAccentColor();
            //            }
            //        }
            //        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            //        {
            //            TryLoadMacOSAccentColor();
            //        }
            //        else
            //        {
            //            LoadDefaultAccentColor();
            //        }

            //    }
            //    else
            //    {
            //        LoadDefaultAccentColor();
            //    }

            //    return;
            //}

            //Color2 col = _customAccentColor.Value;
            //AddOrUpdateSystemResource("SystemAccentColor", (Color)_customAccentColor.Value);

            //AddOrUpdateSystemResource("SystemAccentColorLight1", (Color)col.LightenPercent(0.15f));
            //AddOrUpdateSystemResource("SystemAccentColorLight2", (Color)col.LightenPercent(0.30f));
            //AddOrUpdateSystemResource("SystemAccentColorLight3", (Color)col.LightenPercent(0.45f));

            //AddOrUpdateSystemResource("SystemAccentColorDark1", (Color)col.LightenPercent(-0.15f));
            //AddOrUpdateSystemResource("SystemAccentColorDark2", (Color)col.LightenPercent(-0.30f));
            //AddOrUpdateSystemResource("SystemAccentColorDark3", (Color)col.LightenPercent(-0.45f));

        }


        public void GetAccentColor()
        {
            try
            {
                //UpdateAccentColor("SystemAccentColor", (Color)settings3.GetColorValue(UIColorType.Accent));

                //UpdateAccentColor("SystemAccentColorLight1", (Color)settings3.GetColorValue(UIColorType.AccentLight1));
                //UpdateAccentColor("SystemAccentColorLight2", (Color)settings3.GetColorValue(UIColorType.AccentLight2));
                //UpdateAccentColor("SystemAccentColorLight3", (Color)settings3.GetColorValue(UIColorType.AccentLight3));

                //UpdateAccentColor("SystemAccentColorDark1", (Color)settings3.GetColorValue(UIColorType.AccentDark1));
                //UpdateAccentColor("SystemAccentColorDark2", (Color)settings3.GetColorValue(UIColorType.AccentDark2));
                //UpdateAccentColor("SystemAccentColorDark3", (Color)settings3.GetColorValue(UIColorType.AccentDark3));
            }
            catch
            {
                UpdateAccentColor("SystemAccentColor", Colors.SlateBlue);

                UpdateAccentColor("SystemAccentColorLight1", Color.Parse("#7F69FF"));
                UpdateAccentColor("SystemAccentColorLight2", Color.Parse("#9B8AFF"));
                UpdateAccentColor("SystemAccentColorLight3", Color.Parse("#B9ADFF"));

                UpdateAccentColor("SystemAccentColorDark1", Color.Parse("#43339C"));
                UpdateAccentColor("SystemAccentColorDark2", Color.Parse("#33238C"));
                UpdateAccentColor("SystemAccentColorDark3", Color.Parse("#1D115C"));
            }
        }

        public void UpdateAccentColor(object key, object value)
        {
            var rd = (ResourceDictionary)AvaloniaXamlLoader.Load(new($"avares://wonderlab.control/Theme/BasicResource.axaml"));

            if (rd.ContainsKey(key)) {           
                rd[key] = value;
            }
            else {           
                rd.Add(key, value);
            }
        }

        public void UpdateTheme(string type) {
            var rd = (ResourceDictionary)AvaloniaXamlLoader.Load(new($"avares://wonderlab.control/Theme/{Current}Resource.axaml"));
            Application.Current.Resources.MergedDictionaries.Remove(rd);
            Application.Current.Resources.MergedDictionaries.Add((ResourceDictionary)AvaloniaXamlLoader.Load(new($"avares://wonderlab.control/Theme/{type}Resource.axaml")));
            Current = type;
        }

        public void Load()
        {
            var rd = (ResourceDictionary)AvaloniaXamlLoader.Load(new($"avares://wonderlab.control/Theme/LightResource.axaml"));
            Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        public string Current = "Dark";
    }
}
