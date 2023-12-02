using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.Setting;

namespace WonderLab.Views.Pages.Setting {
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
