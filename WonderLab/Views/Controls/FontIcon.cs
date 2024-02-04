using Avalonia;
using Avalonia.Controls.Primitives;

namespace WonderLab.Views.Controls;

public class FontIcon : TemplatedControl
{
    public string Glyph
    {
        get => GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public static readonly StyledProperty<string> GlyphProperty =
        AvaloniaProperty.Register<FontIcon, string>(nameof(Glyph), "\uE76E");
}