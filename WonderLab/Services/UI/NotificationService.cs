using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Controls;

namespace WonderLab.Services.UI;

public partial class NotificationService : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<INotification> histoys = new();

    [ObservableProperty]
    private ObservableCollection<INotification> notifications = new();

    public void Info(string message, bool canCancelled = true)
    {
        var result = Notification.GetNotification("提示", message,
            canCancelled);

        result.Exited += (sender, args) =>
        {
            Notifications.Remove(result);
        };

        Notifications.Add(result);
        Histoys.Add(Notification.GetNotificationData(result));
    }
}