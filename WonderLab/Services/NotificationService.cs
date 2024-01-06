using WonderLab.Views.Controls;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.Services;

public partial class NotificationService : ObservableObject {
    [ObservableProperty]
    private ObservableCollection<INotification> histoys = new();

    [ObservableProperty]
    private ObservableCollection<INotification> notifications = new();

    public void Info(string message,bool canCancelled = true) {
        var result = Notification.GetNotification("提示", message,
            canCancelled);

        result.Exited += (sender, args) => {
            Notifications.Remove(result);
        };

        Notifications.Add(result);
        Histoys.Add(Notification
            .GetNotificationData(result));
    }
}