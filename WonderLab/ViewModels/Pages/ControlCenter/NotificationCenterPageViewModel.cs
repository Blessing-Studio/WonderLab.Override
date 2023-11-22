using WonderLab.Classes.Managers;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace WonderLab.ViewModels.Pages.ControlCenter {
    public class NotificationCenterPageViewModel : ViewModelBase {
        private NotificationManager _notificationManager;

        public ObservableCollection<INotification> Notifications =>
            _notificationManager.Histoys;

        public NotificationCenterPageViewModel(NotificationManager manager) {
            _notificationManager = manager;
        }
    }
}
