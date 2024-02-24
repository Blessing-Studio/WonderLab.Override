using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using WonderLab.Views.Windows;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly MainWindow window = App.ServiceProvider.GetRequiredService<MainWindow>();
    // private readonly INavigationService _navigationService;
    public string? Title
    {
        get => window.GetValue(Window.TitleProperty);
        set => window.SetValue(Window.TitleProperty, value);
    }
    [ObservableProperty]
    private object? _activePage;

    [ObservableProperty]
    private bool _isOpenBackgroundPanel;

    public MainWindowViewModel()
    {
        if (Debugger.IsAttached)
        {
            window.Title += " - Debugging";
        }
    }

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
    private void NavigationTo(string pageKey)
    {
        IsOpenBackgroundPanel = pageKey switch
        {
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