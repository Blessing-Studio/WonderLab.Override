using Avalonia;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;

using INotification = WonderLab.Classes.Interfaces.INotification;

namespace WonderLab.Views.Controls;

public sealed class NotificationListPanel : TemplatedControl {
    public static readonly StyledProperty<IEnumerable<INotification>> NotificationsProperty =
        AvaloniaProperty.Register<GameManagerPanel, IEnumerable<INotification>>(nameof(Notifications), []);

    public IEnumerable Notifications {
        get => GetValue(NotificationsProperty);
        set => SetValue(NotificationsProperty, value);
    }
}