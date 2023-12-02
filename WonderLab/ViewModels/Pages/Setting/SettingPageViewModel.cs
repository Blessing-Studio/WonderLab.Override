using ReactiveUI.Fody.Helpers;
using WonderLab.Views.Pages.Setting;

namespace WonderLab.ViewModels.Pages.Setting {
    public class SettingPageViewModel : ViewModelBase {
        public SettingPageViewModel(LaunchSettingPage page) {
            Current = page;
        }

        [Reactive]
        public object Current {  get; set; }
    }
}
