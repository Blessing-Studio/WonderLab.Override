using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

using INotification = WonderLab.Classes.Interfaces.INotification;

namespace WonderLab.Views.Controls;

public sealed class NotificationListPanel : TemplatedControl {
    private ListBox _notificationListBox;

    public static readonly StyledProperty<IEnumerable<INotification>> NotificationsProperty =
        AvaloniaProperty.Register<GameManagerPanel, IEnumerable<INotification>>(nameof(Notifications), []);

    public IEnumerable Notifications {
        get => GetValue(NotificationsProperty);
        set => SetValue(NotificationsProperty, value);
    }
}