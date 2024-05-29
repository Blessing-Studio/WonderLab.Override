using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;

namespace WonderLab.Classes.Interfaces;

public interface INotification {
    string Title { get; }
    string Content { get; }
    bool IsCardOpen { set; get; }
    IRelayCommand JumpCommand { get; }
    IRelayCommand CloseButtonCommand { get; }
    NotificationType NotificationType { get; }
}
