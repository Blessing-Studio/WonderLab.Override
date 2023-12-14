using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Views.Pages.Setting;

namespace WonderLab.ViewModels.Pages.Setting {
    public partial class SettingPageViewModel : ViewModelBase {
        public SettingPageViewModel(LaunchSettingPage page) {
            Current = page;
        }

        [ObservableProperty]
        public object current;
    }
}
