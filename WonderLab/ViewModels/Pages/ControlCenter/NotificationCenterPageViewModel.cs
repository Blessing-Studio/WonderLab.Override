using System.Collections.ObjectModel;
using WonderLab.Classes.Interfaces;
using NotificationService = WonderLab.Services.UI.NotificationService;

namespace WonderLab.ViewModels.Pages.ControlCenter;

public sealed class NotificationCenterPageViewModel(NotificationService notificationService) : ViewModelBase
{
    public ObservableCollection<INotification> Notifications =>
        notificationService.Histoys;
}