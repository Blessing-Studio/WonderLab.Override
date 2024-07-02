using Avalonia;
using Avalonia.Controls;
using WonderLab.Services.UI;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Views.Controls;

public sealed class SuperTitleBar : ContentControl {
    private WindowService _windowService;
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SuperTitleBar, string>(nameof(Title), "WonderLab");

    public static readonly StyledProperty<bool> IsButtonGroupVisibleProperty =
        AvaloniaProperty.Register<SuperTitleBar, bool>(nameof(IsButtonGroupVisible), true);

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

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
        
        e.NameScope.Find<Button>("CloseButton")!.Click += (_, _) => _windowService.Close();
        e.NameScope.Find<Button>("MinimizedButton")!.Click += (_, _) => _windowService.SetWindowState(WindowState.Minimized);
    }
}