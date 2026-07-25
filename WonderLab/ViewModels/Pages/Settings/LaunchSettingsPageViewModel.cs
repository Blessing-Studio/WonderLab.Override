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
    public partial bool IsCustomSizeCardVisible { get; set; } = Settings.GameWindowType == GameWindowTypes.Windowed;
    
    public LaunchSettingsPageViewModel(INavigationService navigationService, SettingsService settingsService) : base(navigationService) {
        Settings = settingsService;
    }

    [RelayCommand]
    private void ChangeGameWindowType() {
        IsCustomSizeCardVisible = Settings.GameWindowType == GameWindowTypes.Windowed;
    }
}