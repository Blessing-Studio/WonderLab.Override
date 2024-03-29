using Avalonia;
using Avalonia.Controls;
using System.Collections;
using System.Windows.Input;
using Avalonia.Collections;
using WonderLab.Services.UI;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Input;
using System;

namespace WonderLab.Views.Controls;

public sealed class NavigationView : ContentControl {
    private Frame _frame;
    private Frame _panelFrame;   
    private Border _backgroundPanel;
    private WindowService _windowService;
    private double ActualPx => _backgroundPanel.Bounds.Height + 15;
    
    public static readonly StyledProperty<IEnumerable> MenuItemsProperty =
        AvaloniaProperty.Register<NavigationView, IEnumerable>(nameof(MenuItems),new AvaloniaList<NavigationViewItem>());

    public static new readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(Content));

    public static readonly StyledProperty<object> FooterContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(FooterContent));
    
    public static readonly StyledProperty<bool> IsOpenBackgroundPanelProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsOpenBackgroundPanel));
    
    public IEnumerable MenuItems {
        get => GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    public new object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsOpenBackgroundPanel {
        get => GetValue(IsOpenBackgroundPanelProperty);
        set => SetValue(IsOpenBackgroundPanelProperty, value);
    }
    
    public object FooterContent {
        get => GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    private void SetContent(object page) {
        if (_frame is null || _panelFrame is null) {
            return;
        }

        _frame.Content = null;
        _panelFrame.Content = null;
        
        var panel = IsOpenBackgroundPanel ? _panelFrame : _frame;
        panel.Content = page;
    }
    
    private void SetBackgroundPanelState() {
        var px = IsOpenBackgroundPanel ? 0 : ActualPx;
        _backgroundPanel!.RenderTransform = TransformOperations.Parse($"translateY({px}px)");
    }
    
    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        _windowService = App.ServiceProvider.GetRequiredService<WindowService>();
        _windowService.HandlePropertyChanged(BoundsProperty, SetBackgroundPanelState);
        SetBackgroundPanelState();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        //Frames
        _frame = e.NameScope.Find<Frame>("Frame")!;
        _panelFrame = e.NameScope.Find<Frame>("PanelFrame")!;
        
        //Buttons
        //e.NameScope.Find<Button>("CloseButton")!.Click += (_, _) => _windowService.Close();
        //e.NameScope.Find<Button>("MinimizedButton")!.Click += (_, _) => _windowService.SetWindowState(WindowState.Minimized);

        //Layouts
        _backgroundPanel = e.NameScope.Find<Border>("BackgroundPanel")!;
        //e.NameScope.Find<Border>("Layout")!.PointerPressed += (_, args) => _windowService.BeginMoveDrag(args);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty) {
            SetContent(change.NewValue!);
        }
        
        if (change.Property == IsOpenBackgroundPanelProperty) {
            SetBackgroundPanelState();
        }
    }
}

public sealed class NavigationViewItem : ListBoxItem, ICommandSource {
    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<NavigationViewItem, string>(nameof(Icon));

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<NavigationViewItem, ICommand>(nameof(Command));

    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(CommandParameter));
    
    public string Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand Command {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    public object CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        e.NameScope.Find<Button>("ButtonLayout")!.Click += (sender, args) => {
            IsSelected = IsSelected ? IsSelected : !IsSelected;
        };
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) {
        throw new NotImplementedException();
    }
}