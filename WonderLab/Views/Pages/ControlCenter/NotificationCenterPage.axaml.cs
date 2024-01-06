using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Pages.ControlCenter;

namespace WonderLab.Views.Pages.ControlCenter {
    public partial class NotificationCenterPage : UserControl {
        public NotificationCenterPageViewModel ViewModel { get; set; }

        public NotificationCenterPage() {
            InitializeComponent();
            DataContext = ViewModel = App.ServiceProvider.GetService<NotificationCenterPageViewModel>();
        }
    }
}
