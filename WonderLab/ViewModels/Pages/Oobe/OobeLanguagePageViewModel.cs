using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Services;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class OobeLanguagePageViewModel : ViewModelBase {
    private readonly SettingService _settingService;
    private readonly LanguageService _languageService;

    [ObservableProperty] private int _languageIndex;

    public OobeLanguagePageViewModel(LanguageService languageService, SettingService settingService) {
        _settingService = settingService;
        _languageService = languageService;
    }

    partial void OnLanguageIndexChanged(int value) {
        _languageService.SetLanguage(value);
        _settingService.Data.LanguageIndex = value;
    }

    [RelayCommand]
    private void Navigation() {
        WeakReferenceMessenger.Default.Send(new OobePageMessage {
            PageKey = "OOBEAccount"
        });
    }
}