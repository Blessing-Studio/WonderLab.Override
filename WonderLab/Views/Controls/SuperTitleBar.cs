using Avalonia;
using Avalonia.Controls;
using WonderLab.Services.UI;
using Avalonia.Interactivity;
using MinecraftLaunch.Utilities;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls.Notifications;

namespace WonderLab.Views.Controls;

public sealed class SuperTitleBar : ContentControl {
    private WindowService _windowService;
    
    public static readonly StyledProperty<bool> IsButtonGroupVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsButtonGroupVisible), EnvironmentUtil.IsWindow);
    
    public bool IsButtonGroupVisible {
        get => GetValue(IsButtonGroupVisibleProperty);
        set => SetValue(IsButtonGroupVisibleProperty, value);
    }
    
    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        _windowService = App.ServiceProvider.GetRequiredService<WindowService>();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        
        e.NameScope.Find<Border>("Layout")!.PointerPressed += (_, args) => _windowService.BeginMoveDrag(args);
        var titleTextBlock = e.NameScope.Find<TextBlock>("TitleTextBlock");
        
        if (EnvironmentUtil.IsWindow) {
            e.NameScope.Find<Button>("CloseButton")!.Click += (_, _) => _windowService.Close();
            e.NameScope.Find<Button>("MinimizedButton")!.Click += (_, _) => _windowService.SetWindowState(WindowState.Minimized);
        } else {
            IsVisible = false;
        }
    }
}