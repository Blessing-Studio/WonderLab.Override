using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;

using INotification = WonderLab.Classes.Interfaces.INotification;
using System;

namespace WonderLab.Classes.Datas.ViewData;

public sealed partial class NotificationViewData : ObservableObject, INotification {
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _content;
    [ObservableProperty] private bool _isCardOpen;
    [ObservableProperty] private Action _jumpAction;
    [ObservableProperty] private Action _closeButtonAction;
    [ObservableProperty] private NotificationType _notificationType;

    [RelayCommand] private void Jump() => JumpAction?.Invoke();
    [RelayCommand] private void CloseButton() => CloseButtonAction();
}