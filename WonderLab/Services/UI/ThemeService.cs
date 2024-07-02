using Avalonia;
using Avalonia.Styling;

namespace WonderLab.Services.UI;

public sealed class ThemeService {
    private readonly string DINPro = "resm:WonderLab.Assets.Fonts.DinPro.ttf?assembly=WonderLab#DIN Pro";

    public void SetCurrentTheme(int index) {
        Application.Current.RequestedThemeVariant = index switch {
            0 => ThemeVariant.Light,
            1 => ThemeVariant.Dark,
            2 => ThemeVariant.Default,
            _ => ThemeVariant.Default,
        };
    }

    public void ApplyDefaultFont(string fontName) {
        Application.Current.Resources["DefaultFontFamily"] = $"{DINPro}";
    }
}