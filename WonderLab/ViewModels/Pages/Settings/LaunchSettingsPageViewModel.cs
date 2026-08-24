using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Enums;
using WonderLab.Interfaces.Navigation;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Settings;

public sealed partial class LaunchSettingsPageViewModel : ViewModelBase {
    public SettingsService Settings { get; }

    [ObservableProperty]
    public partial bool IsCustomSizeCardVisible { get; set; }
    
    public LaunchSettingsPageViewModel(INavigationService navigationService, SettingsService settingsService) : base(navigationService) {
        Settings = settingsService;
        IsCustomSizeCardVisible = Settings.GameWindowType == GameWindowTypes.Windowed; // 你初始化的时候还没 Settings 就用啊（恼）
    }

    [RelayCommand]
    private void ChangeGameWindowType() {
        IsCustomSizeCardVisible = Settings.GameWindowType == GameWindowTypes.Windowed;
    }
}