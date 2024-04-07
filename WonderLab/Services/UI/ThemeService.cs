using Avalonia;
using Avalonia.Styling;

namespace WonderLab.Services.UI;

public sealed class ThemeService {
    public void SetCurrentTheme(int index) {
        Application.Current.RequestedThemeVariant = index switch {
            0 => ThemeVariant.Light,
            1 => ThemeVariant.Dark,
            2 => ThemeVariant.Default,
            _ => ThemeVariant.Default,
        };
    }
}