using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Datas;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Download;

namespace WonderLab.ViewModels.Pages.Navigation;

public sealed partial class DownloadNavigationPageViewModel : ViewModelBase {
    private readonly INavigationService _navigationService;

    [ObservableProperty] private NavigationPageData _activePage;

    public DownloadNavigationPageViewModel(DownloadNavigationService navigationService, Dispatcher dispatcher) {
        navigationService.NavigationRequest += async p => {
            await dispatcher.InvokeAsync(() => {
                if (ActivePage?.PageKey != p.PageKey) {
                    ActivePage = p;
                }
            });
        };

        _navigationService = navigationService;
        _navigationService.NavigationTo<SearchPageViewModel>();
    }

    [RelayCommand]
    private void NavigationTo(string pageKey) {
        switch (pageKey) {
            case "SearchPage":
                _navigationService.NavigationTo<SearchPageViewModel>();
                break;
            default:
                _navigationService.NavigationTo<SearchPageViewModel>();
                break;
        }
    }
}