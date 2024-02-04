using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Views.Pages.Setting;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class SettingPageViewModel : ViewModelBase
{
    public SettingPageViewModel(LaunchSettingPage page)
    {
        Current = page;
    }

    [ObservableProperty]
    private object current;
}