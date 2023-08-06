using Avalonia.Platform;
using Avalonia;
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
            using var stream = AssetLoader.Open(new Uri($"resm:wonderlab.Assets.{uri}"));
            stream!.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
