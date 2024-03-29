using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Setting;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Classes.Datas;
using Avalonia.Threading;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Pages.Navigation;

public partial class SettingNavigationPageViewModel : ViewModelBase {
    private readonly INavigationService _navigationService;
    private readonly ControlService _controlService;

    [ObservableProperty] private object _activeItem;
    [ObservableProperty] private NavigationPageData _activePage;

    public SettingNavigationPageViewModel(SettingNavigationService navigationService, ControlService controlService) {
        navigationService.NavigationRequest += p => {
            Dispatcher.Post(() => {
                if (ActivePage?.PageKey != p.PageKey) {
                    ActivePage = p;
                }
            }, DispatcherPriority.ApplicationIdle);
        };

        _controlService = controlService;
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