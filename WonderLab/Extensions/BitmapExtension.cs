using System.IO;
using SixLabors.ImageSharp;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace WonderLab.Extensions;
public static class BitmapExtension {
    public static Bitmap ToBitmap<TPixel>(this Image<TPixel> raw) where TPixel : unmanaged, IPixel<TPixel> {
        using var stream = new MemoryStream();
        raw.Save(stream, new PngEncoder());
        stream.Position = 0;
        return new Bitmap(stream);
    }
}
