using System;
using Avalonia;
using System.Threading;
using Avalonia.Controls;
using System.Collections;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Interactivity;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Classes.Media.Animations;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services.UI;

namespace WonderLab.Views.Controls;

[PseudoClasses(":fullscreen")]
public class NavigationView : TemplatedControl {
    private int _oldIndex;
    private bool _isSwitched;
    private ContentPresenter? _leftContentPresenter;
    private ContentPresenter? _rightContentPresenter;
    private NavigationService _navigationService;
    
    private CancellationTokenSource _token = new();
        
    private readonly PageSlideFade _pageSlideFade = new(TimeSpan.FromMilliseconds(500)) {
        Fade = true
    };
        
    public static readonly StyledProperty<IEnumerable> MenuItemsProperty =
        AvaloniaProperty.Register<NavigationView, IEnumerable>(nameof(MenuItems),new AvaloniaList<NavigationViewItem>());

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(Content));

    public static readonly StyledProperty<bool> IsFullScreenProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsFullScreen));

    public IEnumerable MenuItems {
        get => GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsFullScreen {
        get => GetValue(IsFullScreenProperty);
        set => SetValue(IsFullScreenProperty, value);
    }

    public NavigationView() {
        UpdatePseudoClasses(false);
        _navigationService = App.ServiceProvider.GetService<NavigationService>()!;
        WeakReferenceMessenger.Default.Register<PageMessage>(this, (x, x1) => {
            if (!x1.IsChildrenPage) {
                Content = x1.Page;
            }
        });
    }

    private void UpdatePseudoClasses(bool? isFullScreen) {
        if (isFullScreen.HasValue) {
            PseudoClasses.Set(":fullscreen", isFullScreen.Value);
        }
    }
        
    private async void RunPageTransitionAnimation() {
        if (_leftContentPresenter is null || _rightContentPresenter is null) {
            return;
        }

        using (_token) {
            _token.Cancel();
            _token = new();
        }

        if (_isSwitched) {
            _rightContentPresenter.Content = Content;
            await _pageSlideFade.Start(_leftContentPresenter,
                _rightContentPresenter,
                false,
                _token.Token);
        }
        else {
            _leftContentPresenter.Content = Content;
            await _pageSlideFade.Start(_rightContentPresenter,
                _leftContentPresenter,
                true,
                _token.Token);
        }

        _isSwitched = !_isSwitched;
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        _leftContentPresenter!.Content = Content;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _leftContentPresenter = e.NameScope
            .Find<ContentPresenter>("LeftContent");
            
        _rightContentPresenter = e.NameScope
            .Find<ContentPresenter>("RightContent");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);
            
        if (change.Property == IsFullScreenProperty) {
            UpdatePseudoClasses((bool)change.NewValue!);
        }

        if (change.Property == ContentProperty) {
            RunPageTransitionAnimation();
        }
    }
}

public class NavigationViewItem : ListBoxItem {
    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<NavigationViewItem, string>(nameof(Icon));

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<NavigationViewItem, ICommand>(nameof(Command));

    public string Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand Command {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        e.NameScope.Find<Button>("ButtonLayout")!.Click += (sender, args) => {
            IsSelected = IsSelected ? IsSelected : !IsSelected;
        };
    }
}