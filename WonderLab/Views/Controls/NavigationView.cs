using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Threading;
using System.Windows.Input;
using System.Threading.Tasks;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;
using Avalonia.Media.Transformation;

namespace WonderLab.Views.Controls;

[PseudoClasses(":ispanelopen", ":ispanelclose")]
public sealed class NavigationView : SelectingItemsControl {
    public sealed class NavigationViewTemplateSettings : AvaloniaObject {
        private double _actualPx;

        public static readonly DirectProperty<NavigationViewTemplateSettings, double> ActualPxProperty =
                AvaloniaProperty.RegisterDirect<NavigationViewTemplateSettings, double>(nameof(ActualPx), p => p.ActualPx,
                    (p, o) => p.ActualPx = o);

        public double ActualPx {
            get => _actualPx;
            set => SetAndRaise(ActualPxProperty, ref _actualPx,
                value);
        }
    }

    private Frame PART_Frame;
    private Frame PART_PanelFrame;
    private LayoutTransformControl PART_LayoutTransformControl;

    private Border _backgroundPanel;
    private bool _oldIsOpenBackgroundPanel;

    public NavigationViewTemplateSettings TemplateSettings { get; } = new();

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(Content));

    public static readonly StyledProperty<object> PanelContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(PanelContent));

    public static readonly StyledProperty<object> FooterContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(FooterContent));
    
    public static readonly StyledProperty<bool> IsOpenBackgroundPanelProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsOpenBackgroundPanel));

    [Content]
    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public object PanelContent {
        get => GetValue(PanelContentProperty);
        set => SetValue(PanelContentProperty, value);
    }

    public object FooterContent {
        get => GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    public bool IsOpenBackgroundPanel {
        get => GetValue(IsOpenBackgroundPanelProperty);
        set => SetValue(IsOpenBackgroundPanelProperty, value);
    }

    private DispatcherOperation RunAnimation() {
        var px = IsOpenBackgroundPanel ? 0 : _backgroundPanel.Bounds.Height + 15;

        Dispatcher.UIThread.VerifyAccess();
        return Dispatcher.UIThread.InvokeAsync(() => {
            PART_LayoutTransformControl.Opacity = IsOpenBackgroundPanel ? 1 : 0;
            PART_LayoutTransformControl.RenderTransform = TransformOperations.Parse($"translateY({px}px)");
        });

    }

    protected override async void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        //Layouts
        PART_Frame = e.NameScope.Find<Frame>("PART_Frame");
        PART_PanelFrame = e.NameScope.Find<Frame>("PART_PanelFrame");
        PART_LayoutTransformControl = e.NameScope.Find<LayoutTransformControl>("PART_LayoutTransformControl");
        _backgroundPanel = e.NameScope.Find<Border>("BackgroundPanel");

        await Dispatcher.UIThread.InvokeAsync(() => {
            PART_LayoutTransformControl.Opacity = 0;
            PART_LayoutTransformControl.RenderTransform = TransformOperations.Parse($"translateY({_backgroundPanel.Bounds.Height + 15}px)");
        });
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenBackgroundPanelProperty) {
            await RunAnimation();

            await Task.Delay(TimeSpan.Parse("0:0:0.3"));
            await Dispatcher.UIThread.InvokeAsync(() => PART_PanelFrame.Content = PanelContent, DispatcherPriority.ApplicationIdle);
        }

        if (change.Property == PanelContentProperty) {
            var dispatcherOperation = RunAnimation();

            dispatcherOperation.Completed += async (_, _) => {
                await Task.Delay(TimeSpan.Parse("0:0:0.3"));
                await Dispatcher.UIThread.InvokeAsync(() => PART_PanelFrame.Content = PanelContent, DispatcherPriority.ApplicationIdle);
            };

            await dispatcherOperation;
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