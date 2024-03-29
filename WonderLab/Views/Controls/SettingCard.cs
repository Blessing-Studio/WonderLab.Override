using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using System.Collections;

namespace WonderLab.Views.Controls;

public sealed class SettingCard : ContentControl {
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SettingCard, string>(nameof(Title), "Hello Title");

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}