using Avalonia;
using Avalonia.Controls;
using WonderLab.Services.UI;
using Avalonia.Interactivity;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Views.Controls;

public sealed class DoubleSplitView : ContentControl {
    private WindowService _windowService;
    private ContentPresenter _rightPanel;

    public bool IsPaneOpen {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public object PanelContent {
        get => GetValue(PanelContentProperty);
        set => SetValue(PanelContentProperty, value);
    }

    public static readonly StyledProperty<object> PanelContentProperty =
        AvaloniaProperty.Register<DoubleSplitView, object>(nameof(PanelContent));

    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<DoubleSplitView, bool>(nameof(IsPaneOpen), false);

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        _windowService = App.ServiceProvider.GetRequiredService<WindowService>();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        _rightPanel = e.NameScope.Find<ContentPresenter>("rightPanel");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (_rightPanel is null) {
            return;
        }

        if (change.Property == IsPaneOpenProperty) {
            if (IsPaneOpen) {
                var actulPanelWidth = _windowService.ActualWidth / 3;
                _rightPanel.Width = actulPanelWidth;
                return;
            }

            _rightPanel.Width = 0;
        }

        //if (change.Property == BoundsProperty) {
        //    if (IsPaneOpen) {
        //        var actulPanelWidth = this.Bounds.Width / 3;
        //        _rightPanel.Width = actulPanelWidth;
        //        return;
        //    }
        //}
    }
}