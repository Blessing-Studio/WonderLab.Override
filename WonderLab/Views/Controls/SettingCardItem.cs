using Avalonia;
using Avalonia.Controls;

namespace WonderLab.Views.Controls;

public sealed class SettingCardItem : ContentControl {
    public string Description {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    
    public static readonly StyledProperty<string> DescriptionProperty = 
        AvaloniaProperty.Register<RollingBorder, string>(nameof(Description), "Hello");
}
