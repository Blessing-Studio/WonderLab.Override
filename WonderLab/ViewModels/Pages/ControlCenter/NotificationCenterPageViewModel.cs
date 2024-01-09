using WonderLab.Services;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;
using NotificationService = WonderLab.Services.UI.NotificationService;

namespace WonderLab.ViewModels.Pages.ControlCenter;

public class NotificationCenterPageViewModel(NotificationService notificationService) : ViewModelBase {
    public ObservableCollection<INotification> Notifications =>
        notificationService.Histoys;
}