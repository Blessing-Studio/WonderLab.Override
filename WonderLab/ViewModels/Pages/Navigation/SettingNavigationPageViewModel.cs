using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Setting;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Pages.Navigation;

public partial class SettingNavigationPageViewModel : ViewModelBase {
    private readonly INavigationService _navigationService;

    [ObservableProperty] private object? _activePage;

    public SettingNavigationPageViewModel(SettingNavigationService navigationService) {
        navigationService.NavigationRequest += p => {
            ActivePage = null;
            ActivePage = p;
        };

        _navigationService = navigationService;
        _navigationService.NavigationTo<LaunchSettingPageViewModel>();
    }
    
    [RelayCommand]
    private void NavigationTo(string pageKey) {
        switch (pageKey) {
            case "LaunchSettingPage":
                _navigationService.NavigationTo<LaunchSettingPageViewModel>(); 
                break;
            case "AccountSettingPage":
                _navigationService.NavigationTo<AccountSettingPageViewModel>(); 
                break;
            case "NetworkSettingPage":
                _navigationService.NavigationTo<NetworkSettingPageViewModel>(); 
                break;
            default:
                _navigationService.NavigationTo<LaunchSettingPageViewModel>();
                break;
        }
    }
}