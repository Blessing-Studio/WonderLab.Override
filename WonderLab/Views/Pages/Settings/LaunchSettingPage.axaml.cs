using Avalonia.Controls;
using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.Settings;

namespace WonderLab.Views.Pages.Settings {
    public partial class LaunchSettingPage : ReactiveUserControl<LaunchSettingPageViewModel> {
        public LaunchSettingPage(LaunchSettingPageViewModel vm) {
            InitializeComponent();
            ViewModel = vm;
        }

        public LaunchSettingPage() {
            InitializeComponent();
        }
    }
}
