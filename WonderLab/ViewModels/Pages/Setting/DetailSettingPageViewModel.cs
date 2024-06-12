using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Classes.Enums;
using WonderLab.Services;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class DetailSettingPageViewModel : ViewModelBase {
    private readonly ThemeService _themeService;
    private readonly WindowService _windowService;
    private readonly SettingService _settingService;
    private readonly LanguageService _languageService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private int _themeIndex = 0;
    [ObservableProperty] private int _languageIndex = 0;
    [ObservableProperty] private int _backgroundIndex = 0;
    [ObservableProperty] private bool _isDebugMode = false;

    public DetailSettingPageViewModel(
        ThemeService themeService, 
        WindowService windowService,
        SettingService settingService, 
        LanguageService languageService,
        NotificationService notificationService) {
        _themeService = themeService;
        _windowService = windowService;
        _settingService = settingService;
        _languageService = languageService;
        _notificationService = notificationService;

        ThemeIndex = _settingService.Data.ThemeIndex;
        IsDebugMode = _settingService.Data.IsDebugMode;
        LanguageIndex = _settingService.Data.LanguageIndex;
        BackgroundIndex = _settingService.Data.BackgroundIndex;
    }

    [RelayCommand]
    private void Notification(string text) {
        _notificationService.QueueJob(new NotificationViewData {
            Title = "пео╒",
            Content = text,
            NotificationType = NotificationType.Information
        });
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
            case nameof(LanguageIndex):
                _languageService.SetLanguage(LanguageIndex);
                _settingService.Data.LanguageIndex = LanguageIndex;
                break;
            case nameof(IsDebugMode):
                _settingService.Data.IsDebugMode = IsDebugMode;
                break;
        }
    }
}