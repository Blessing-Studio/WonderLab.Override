using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Views.Controls {
    public class FontIcon : TemplatedControl {
        public string Glyph { get => GetValue(GlyphProperty); set => SetValue(GlyphProperty, value); }

        public static readonly StyledProperty<string> GlyphProperty =
            AvaloniaProperty.Register<FontIcon, string>(nameof(Glyph), "\uE76E");

        public FontIcon() {}

        public FontIcon(string iconGlyph) {
            Glyph = iconGlyph;
        }
    }
}
