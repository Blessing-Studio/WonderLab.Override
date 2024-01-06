using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.ControlCenter {
    public class NotificationCenterPageViewModel : ViewModelBase {
        private NotificationService _notificationService;

        public ObservableCollection<INotification> Notifications =>
            _notificationService.Histoys;

        public NotificationCenterPageViewModel(NotificationService notificationService) {
            _notificationService = notificationService;
        }
    }
}
