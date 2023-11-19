using Avalonia.Controls;
using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.Settings;

namespace WonderLab.Views.Pages.Settings {
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
