using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.Navigation;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
   private readonly INavigationService _navigationService;

    [ObservableProperty] 
    private object? _activePage;

    [ObservableProperty]
    private bool _isOpenBackgroundPanel;
    
    public MainWindowViewModel(HostNavigationService navigationService) {
        navigationService.NavigationRequest += p => {
            Dispatcher.Post(() => {
                ActivePage = p;
            }, DispatcherPriority.ApplicationIdle);
        };
        
        _navigationService = navigationService;
        _navigationService.NavigationTo<HomePageViewModel>();
    }


    [RelayCommand]
    private void NavigationTo(string pageKey) {
        IsOpenBackgroundPanel = pageKey switch {
            "HomePage" => false,
            "SettingPage" => true,
            "DownloadPage" => true,
            _ => false
        };
        
        switch (pageKey) {
            case "HomePage":
                _navigationService.NavigationTo<HomePageViewModel>(); 
                break;
            case "SettingPage":
                _navigationService.NavigationTo<SettingNavigationPageViewModel>(); 
                break;
            case "DownloadPage":
                _navigationService.NavigationTo<DownloadNavigationPageViewModel>(); 
                break;
            default:
                _navigationService.NavigationTo<HomePageViewModel>();
                break;
        }
    }
}