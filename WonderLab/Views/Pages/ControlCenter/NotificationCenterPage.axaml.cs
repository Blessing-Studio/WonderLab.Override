using Avalonia.Controls;
using WonderLab.ViewModels.Pages.ControlCenter;

namespace WonderLab.Views.Pages.ControlCenter {
    public partial class NotificationCenterPage : UserControl {
        public NotificationCenterPageViewModel ViewModel { get; set; }

        public NotificationCenterPage(NotificationCenterPageViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }

        public NotificationCenterPage() {
            InitializeComponent();
        }
    }
}
