using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.ControlCenter;

namespace WonderLab.Views.Pages.ControlCenter {
    public partial class NotificationCenterPage : ReactiveUserControl<NotificationCenterPageViewModel> {
        public NotificationCenterPage(NotificationCenterPageViewModel vm) {
            InitializeComponent();
            ViewModel = vm;
        }

        public NotificationCenterPage() {
            InitializeComponent();
        }
    }
}
