using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WonderLab.Views.Controls;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace WonderLab.Classes.Managers {
    public class NotificationManager : ReactiveObject {
        [Reactive]
        public ObservableCollection<INotification> Histoys { get; set; }

        [Reactive]
        public ObservableCollection<INotification> Notifications { get; set; }

        public NotificationManager() {
            Histoys = new();
            Notifications = new();
        }

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
}
