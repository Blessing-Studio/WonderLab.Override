using Avalonia;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.Services.UI;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Views.Controls;

public sealed class DialogContentPanel : TemplatedControl {
    private Button _closeButton;
    private Button _topCloseButton;

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<DialogContentPanel, string>(nameof(Title));

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<DialogContentPanel, object>(nameof(Content));

    public static readonly StyledProperty<object> BottomBarContentProperty =
        AvaloniaProperty.Register<DialogContentPanel, object>(nameof(BottomBarContent));

    public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
        AvaloniaProperty.Register<DialogContentPanel, bool>(nameof(IsCloseButtonVisible));

    public static readonly StyledProperty<bool> IsTopCloseButtonVisibleProperty =
        AvaloniaProperty.Register<DialogContentPanel, bool>(nameof(IsTopCloseButtonVisible));

    public static readonly StyledProperty<bool> IsPrimaryButtonVisibleProperty =
        AvaloniaProperty.Register<DialogContentPanel, bool>(nameof(IsPrimaryButtonVisible));

    public static readonly StyledProperty<object> PrimaryButtonContentProperty =
        AvaloniaProperty.Register<DialogContentPanel, object>(nameof(PrimaryButtonContent));

    public static readonly StyledProperty<ICommand> PrimaryButtonCommandProperty =
        AvaloniaProperty.Register<DialogContentPanel, ICommand>(nameof(PrimaryButtonCommand));

    public static readonly StyledProperty<object> PrimaryButtonCommandParameterProperty =
        AvaloniaProperty.Register<DialogContentPanel, object>(nameof(PrimaryButtonCommandParameter));

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public object BottomBarContent {
        get => GetValue(BottomBarContentProperty);
        set => SetValue(BottomBarContentProperty, value);
    }

    public bool IsCloseButtonVisible {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    public object PrimaryButtonContent {
        get => GetValue(PrimaryButtonContentProperty);
        set => SetValue(PrimaryButtonContentProperty, value);
    }

    public bool IsPrimaryButtonVisible {
        get => GetValue(IsPrimaryButtonVisibleProperty);
        set => SetValue(IsPrimaryButtonVisibleProperty, value);
    }

    public bool IsTopCloseButtonVisible {
        get => GetValue(IsTopCloseButtonVisibleProperty);
        set => SetValue(IsTopCloseButtonVisibleProperty, value);
    }

    public ICommand PrimaryButtonCommand {
        get => GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    public object PrimaryButtonCommandParameter {
        get => GetValue(PrimaryButtonCommandParameterProperty);
        set => SetValue(PrimaryButtonCommandParameterProperty, value);
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        var dialogService = App.ServiceProvider.GetService<DialogService>();
        _closeButton.Click += (_, _) => dialogService.CloseContentDialog();
        _topCloseButton.Click += (_, _) => dialogService.CloseContentDialog();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _closeButton = e.NameScope.Find<Button>("closeButton");
        _topCloseButton = e.NameScope.Find<Button>("topCloseButton");
    }
}