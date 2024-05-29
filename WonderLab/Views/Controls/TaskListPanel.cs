using Avalonia;
using Avalonia.Controls;
using System.Collections;
using WonderLab.Services.UI;
using Avalonia.Interactivity;
using System.Collections.Generic;
using WonderLab.Classes.Interfaces;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Views.Controls;

public sealed class TaskListPanel : TemplatedControl {
    private Border _layout;
    private ListBox _taskListBox;
    private Border _contentLayout;
    private TextBlock _taskListTip;
    private WindowService _windowService;

    public bool IsPaneOpen {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public IEnumerable Tasks {
        get => GetValue(TasksProperty);
        set => SetValue(TasksProperty, value);
    }

    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<GameManagerPanel, bool>(nameof(IsPaneOpen), false);

    public static readonly StyledProperty<IEnumerable<ITaskJob>> TasksProperty =
        AvaloniaProperty.Register<GameManagerPanel, IEnumerable<ITaskJob>>(nameof(Tasks), []);

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        if (Design.IsDesignMode) {
            return;
        }

        _windowService = App.ServiceProvider.GetService<WindowService>();
        _contentLayout.RenderTransform = TransformOperations.Parse($"translateX({_contentLayout.Bounds.Width + 10}px)");

        _windowService.HandlePropertyChanged(BoundsProperty, () => {
            if (_taskListBox.ItemCount > 0) {
                _contentLayout.Height = _layout.Bounds.Height - 5;
            }

            _contentLayout.MaxHeight = _layout.Bounds.Height - 5;
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _layout = e.NameScope.Find<Border>("layout");
        _taskListBox = e.NameScope.Find<ListBox>("taskListBox");
        _taskListTip = e.NameScope.Find<TextBlock>("taskListTip");
        _contentLayout = e.NameScope.Find<Border>("contentLayout");

        _taskListBox.Items.CollectionChanged += (o, args) => {
            switch (args.Action) {
                case NotifyCollectionChangedAction.Add:
                    _taskListTip.Opacity = 0;
                    if (_taskListBox.ItemCount == 1) {
                        _contentLayout.Height = _layout.Bounds.Height - 5;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (_taskListBox.ItemCount == 0) {
                        _taskListTip.Opacity = 1;
                        _contentLayout.Height = 130;
                    }
                    break;
            }
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsPaneOpenProperty) {
            var px = change.GetNewValue<bool>() ? 0 : _contentLayout.Bounds.Width + 10;
            _contentLayout.RenderTransform = TransformOperations.Parse($"translateX({px}px)");
        }
    }
}