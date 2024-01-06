using WonderLab.Views.Pages.Setting;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Pages.Setting;

public partial class SettingPageViewModel : ViewModelBase {
    public SettingPageViewModel(LaunchSettingPage page) {
        Current = page;
    }

    [ObservableProperty]
    private object current;
}