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
using Avalonia.Threading;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Controls.Metadata;

namespace WonderLab.Views.Controls;

[PseudoClasses(":ispanelopen", ":ispanelclose")]
public sealed class NavigationView : ContentControl {
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

    private Frame _frame;
    private Frame _panelFrame;   
    private Border _backgroundPanel;
    private WindowService _windowService;
    private CancellationTokenSource _cancellationTokenSource = new();

    //private double ActualPx => _backgroundPanel.Bounds.Height + 15;
    public NavigationViewTemplateSettings TemplateSettings { get; } = new();

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

    private async void SetContent(object page) {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();

        if (_frame is null || _panelFrame is null) {
            return;
        }

        _frame.Content = null;
        _panelFrame.Content = null;
        
        var panel = IsOpenBackgroundPanel ? _panelFrame : _frame;

        await Dispatcher.UIThread.InvokeAsync(async () => {
            panel.Content = page;

            await Task.Delay(390, _cancellationTokenSource.Token).ContinueWith(x => {
                if (x.IsCompletedSuccessfully) {
                    Dispatcher.UIThread.Post(() => panel.Opacity = 1);
                }
            });
        });
    }
    
    private void SetBackgroundPanelState() {
        //var px = IsOpenBackgroundPanel ? 0 : ActualPx;
        //Dispatcher.UIThread.Post(() => {
        //    _backgroundPanel!.RenderTransform = TransformOperations.Parse($"translateY({px}px)");
        //}, DispatcherPriority.Send);
    }

    private void UpdateIndicator() {
        TemplateSettings.ActualPx = _backgroundPanel.Bounds.Height + 15;
    }

    private void UpdatePseudoClasses() {
        PseudoClasses.Set(":ispanelopen", IsOpenBackgroundPanel is true);
        PseudoClasses.Set(":ispanelclose", IsOpenBackgroundPanel is false);
    }

    private void OnPanelPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (e.Property == BoundsProperty) {
            UpdateIndicator();
        }
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        if (Design.IsDesignMode) {
            return;
        }

        _windowService = App.ServiceProvider.GetRequiredService<WindowService>();
        //_windowService.HandlePropertyChanged(BoundsProperty, SetBackgroundPanelState);
        //SetBackgroundPanelState();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        //Frames
        _frame = e.NameScope.Find<Frame>("Frame")!;
        _panelFrame = e.NameScope.Find<Frame>("PanelFrame")!;
        
        //Layouts
        _backgroundPanel = e.NameScope.Find<Border>("BackgroundPanel")!;
        _backgroundPanel.PropertyChanged += OnPanelPropertyChanged;
        //e.NameScope.Find<Border>("Layout")!.PointerPressed += (_, args) => _windowService.BeginMoveDrag(args);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty) {
            SetContent(change.NewValue!);
        }
        
        if (change.Property == IsOpenBackgroundPanelProperty) {
            UpdatePseudoClasses();
            //SetBackgroundPanelState();
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