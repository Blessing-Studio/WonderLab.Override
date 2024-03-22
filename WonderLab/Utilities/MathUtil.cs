using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using System.Globalization;

namespace WonderLab.Utilities;

/// <summary>
/// 额外数学计算工具类
/// </summary>
public static class MathUtil {
    public static Rect CalculateText(string text, TextBlock control) {
         var formattedText = new FormattedText(text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(control.FontFamily, control.FontStyle, control.FontWeight),
            control.FontSize,
            Brushes.Black
         );

        return formattedText.Width > 140
            ? new(0, 0, formattedText.Width + 15, formattedText.Height)
            : new(0, 0, 155, formattedText.Height);
    }
}