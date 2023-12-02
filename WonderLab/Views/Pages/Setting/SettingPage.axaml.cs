using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.Setting;

namespace WonderLab.Views.Pages.Setting {
    public partial class SettingPage : ReactiveUserControl<SettingPageViewModel> {
        public SettingPage(SettingPageViewModel vm) {
            InitializeComponent();
            ViewModel = vm;
        }

        public SettingPage() {
            InitializeComponent();
        }
    }
}
