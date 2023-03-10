using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class BitmapUtils {
        public static IImage GetAssetBitmap(string uri) {
            var al = AvaloniaLocator.Current.GetService<IAssetLoader>();
            using (var s = al.Open(new Uri($"resm:wonderlab.Assets.{uri}"))) {
                return new Bitmap(s);
            }
        }

        public static IImage GetIconBitmap(string uri) {       
            var al = AvaloniaLocator.Current.GetService<IAssetLoader>();
            using (var s = al.Open(new Uri($"avares://wonderlab/Assets/Icons/{uri}")))
            {
                return new Bitmap(s);
            }
        }
    }
}
