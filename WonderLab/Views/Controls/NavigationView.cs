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
using Avalonia.Controls.Presenters;

namespace WonderLab.Views.Controls;

[PseudoClasses(":ispanelopen", ":ispanelclose")]
public sealed class NavigationView : SelectingItemsControl {
    private ContentPresenter _PART_ContentPresenter;
    private Border _PART_Border;

    private bool _isRunPanelAnimation;

    private event EventHandler AnimationCompleted;

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(Content));

    public static readonly StyledProperty<object> FooterContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(FooterContent));
    
    public static readonly StyledProperty<bool> IsOpenBackgroundPanelProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsOpenBackgroundPanel));

    [Content]
    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public object FooterContent {
        get => GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    public bool IsOpenBackgroundPanel {
        get => GetValue(IsOpenBackgroundPanelProperty);
        set => SetValue(IsOpenBackgroundPanelProperty, value);
    }

    private void OnAnimationCompleted(object sender, EventArgs e) {
        _isRunPanelAnimation = true;
        Dispatcher.UIThread.Post(() => _PART_ContentPresenter.Content = Content, DispatcherPriority.ApplicationIdle);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_ContentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        _PART_Border = e.NameScope.Find<Border>("PART_Border");

        AnimationCompleted += OnAnimationCompleted;
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenBackgroundPanelProperty) {
            var @bool = change.GetNewValue<bool>();

            Dispatcher.UIThread.Post(() => {
                _PART_Border.Opacity = @bool ? 1d : 0d;
                _PART_Border.RenderTransform = TransformOperations.Parse($"translateY({(@bool ? 0 : 15)}px)");
            }, DispatcherPriority.Send);

            await Dispatcher.UIThread.InvokeAsync(() => {
                _PART_ContentPresenter.Content = null;
            }, DispatcherPriority.ApplicationIdle);

            await Task.Delay(TimeSpan.Parse("0:0:0.38"));
            AnimationCompleted?.Invoke(this, EventArgs.Empty);
        }

        if (change.Property == ContentProperty) {
            if (_isRunPanelAnimation) {
                _isRunPanelAnimation = false;
                return;
            }

            Dispatcher.UIThread.Post(() => _PART_ContentPresenter.Content = change.NewValue, DispatcherPriority.ApplicationIdle);
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