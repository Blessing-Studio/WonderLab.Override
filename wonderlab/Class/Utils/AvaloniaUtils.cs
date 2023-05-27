using Avalonia.Platform;
using Avalonia;
using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace wonderlab.Class.Utils {
    public static class AvaloniaUtils {
        public static MemoryStream GetAssetsStream(string uri) {
            var memoryStream = new MemoryStream();

            IAssetLoader service = AvaloniaLocator.Current.GetService<IAssetLoader>()!;
            using var stream = service?.Open(new Uri($"resm:wonderlab.Assets.{uri}"));
            stream!.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
