using Avalonia.Controls;
using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.ControlCenter;

namespace WonderLab.Views.Pages.ControlCenter {
    public partial class TaskCenterPage : ReactiveUserControl<TaskCenterPageViewModel> {
        public TaskCenterPage(TaskCenterPageViewModel viewModel) {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public TaskCenterPage() {
            InitializeComponent();
        }
    }
}
