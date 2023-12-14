using Avalonia.Controls;
using WonderLab.ViewModels.Pages.ControlCenter;

namespace WonderLab.Views.Pages.ControlCenter {
    public partial class TaskCenterPage : UserControl {
        public TaskCenterPageViewModel ViewModel { get; set; }

        public TaskCenterPage(TaskCenterPageViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }

        public TaskCenterPage() {
            InitializeComponent();
        }
    }
}
