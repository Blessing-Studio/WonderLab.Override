using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
   // private readonly INavigationService _navigationService;

    [ObservableProperty] 
    private object? _activePage;

    [ObservableProperty]
    private bool _isOpenBackgroundPanel;
    
    // public MainWindowViewModel(HostNavigationService navigationService) {
    //     navigationService.NavigationRequest += p => {
    //         Dispatcher.Post(() => {
    //             ActivePage = p;
    //         }, DispatcherPriority.ApplicationIdle);
    //     };
    //     
    //     _navigationService = navigationService;
    //     _navigationService.NavigationTo<HomePageViewModel>();
    // }

    
    [RelayCommand]
    private void NavigationTo(string pageKey) {
        IsOpenBackgroundPanel = pageKey switch {
            "HomePage" => false,
            "SettingPage" => true,
            "DownloadPage" => true,
            _ => false
        };
        
        // switch (pageKey) {
        //     case "HomePage":
        //         _navigationService.NavigationTo<HomePageViewModel>(); 
        //         break;
        //     case "SettingPage":
        //         _navigationService.NavigationTo<SettingPageViewModel>(); 
        //         break;
        //     case "DownloadPage":
        //         _navigationService.NavigationTo<DownloadPageViewModel>(); 
        //         break;
        //     default:
        //         _navigationService.NavigationTo<HomePageViewModel>();
        //         break;
        // }
    }
}