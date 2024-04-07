using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using WonderLab.Classes.Enums;
using WonderLab.Services;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class DetailSettingPageViewModel : ViewModelBase {
    private readonly ThemeService _themeService;
    private readonly WindowService _windowService;
    private readonly SettingService _settingService;

    [ObservableProperty] private int _themeIndex = 0;
    [ObservableProperty] private int _backgroundIndex = 0;

    public DetailSettingPageViewModel(ThemeService themeService, SettingService settingService, WindowService windowService) {
        _themeService = themeService;
        _windowService = windowService;
        _settingService = settingService;

        ThemeIndex = _settingService.Data.ThemeIndex;
        BackgroundIndex = _settingService.Data.BackgroundIndex;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(ThemeIndex):
                _themeService.SetCurrentTheme(ThemeIndex);
                _settingService.Data.ThemeIndex = ThemeIndex;
                break;
            case nameof(BackgroundIndex):
                _windowService.SetBackground(BackgroundIndex);
                _settingService.Data.BackgroundIndex = BackgroundIndex;
                break;
        }
    }
}