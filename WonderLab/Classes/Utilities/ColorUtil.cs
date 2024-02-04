using Avalonia.Media;

namespace WonderLab.Classes.Utilities;

public static class ColorUtil
{
    public static bool IsDark(this Color color)
    {
        double brightness = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
        return brightness <= 128;
    }
}
