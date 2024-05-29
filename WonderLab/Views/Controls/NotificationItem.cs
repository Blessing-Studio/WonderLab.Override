using Avalonia;
using System.Windows.Input;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Notifications;
using Avalonia.Controls;

namespace WonderLab.Views.Controls;

[PseudoClasses(":error", ":information", ":success", ":warning")]
public sealed class NotificationItem : TemplatedControl {
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<NotificationItem, string>(nameof(Title));

    public static readonly StyledProperty<bool> IsCancelProperty =
        AvaloniaProperty.Register<NotificationItem, bool>(nameof(IsCancel));

    public static readonly StyledProperty<string> ContentProperty =
        AvaloniaProperty.Register<NotificationItem, string>(nameof(Content));

    public static readonly StyledProperty<ICommand> JumpCommandProperty =
        AvaloniaProperty.Register<NotificationItem, ICommand>(nameof(JumpCommand));

    public static readonly StyledProperty<ICommand> CloseCommandProperty =
        AvaloniaProperty.Register<NotificationItem, ICommand>(nameof(CloseCommand));

    public static readonly StyledProperty<NotificationType> NotificationTypeProperty =
        AvaloniaProperty.Register<NotificationItem, NotificationType>(nameof(NotificationType));

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsCancel {
        get => GetValue(IsCancelProperty);
        set => SetValue(IsCancelProperty, value);
    }

    public string Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public ICommand JumpCommand {
        get => GetValue(JumpCommandProperty);
        set => SetValue(JumpCommandProperty, value);
    }

    public ICommand CloseCommand {
        get => GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public NotificationType NotificationType {
        get => GetValue(NotificationTypeProperty);
        set => SetValue(NotificationTypeProperty, value);
    }

    private void UpdateNotificationType() {
        switch (NotificationType) {
            case NotificationType.Error:
                PseudoClasses.Add(":error");
                break;
            case NotificationType.Information:
                PseudoClasses.Add(":information");
                break;
            case NotificationType.Success:
                PseudoClasses.Add(":success");
                break;
            case NotificationType.Warning:
                PseudoClasses.Add(":warning");
                break;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        e.NameScope.Find<LayoutTransformControl>("PART_LayoutTransformControl").PointerPressed += (_, args) => {
            JumpCommand?.Execute(null);
            CloseCommand?.Execute(null);
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == NotificationTypeProperty) {
            UpdateNotificationType();
        }
    }
}