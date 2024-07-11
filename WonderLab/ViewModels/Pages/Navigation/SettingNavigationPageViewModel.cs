using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Setting;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Classes.Datas;
using Avalonia.Threading;
using WonderLab.Services.UI;
using WonderLab.Views.Pages.Setting;

namespace WonderLab.ViewModels.Pages.Navigation;

public sealed partial class SettingNavigationPageViewModel : ViewModelBase {
    private readonly INavigationService _navigationService;

    [ObservableProperty] private object _activeItem;
    [ObservableProperty] private NavigationPageData _activePage;

    public SettingNavigationPageViewModel(SettingNavigationService navigationService, Dispatcher dispatcher) {
        navigationService.NavigationRequest += p => {
            dispatcher.Post(() => {
                if (ActivePage?.PageKey != p.PageKey) {
                    ActivePage = p;
                }
            }, DispatcherPriority.ApplicationIdle);
        };

        _navigationService = navigationService;
        _navigationService.NavigationTo<LaunchSettingPageViewModel>();
    }

    [RelayCommand]
    private void NavigationTo(string pageKey) {
        switch (pageKey) {
            case "AboutPage":
                _navigationService.NavigationTo<AboutPageViewModel>();
                break;
            case "LaunchSettingPage":
                _navigationService.NavigationTo<LaunchSettingPageViewModel>(); 
                break;
            case "DetailSettingPage":
                _navigationService.NavigationTo<DetailSettingPageViewModel>();
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