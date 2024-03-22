﻿using Avalonia.Threading;
using WonderLab.ViewModels.Pages;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.ViewModels.Pages.Navigation;
using Avalonia.Platform;
using MinecraftLaunch.Utilities;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
   private readonly INavigationService _navigationService;

    [ObservableProperty] 
    private object? _activePage;

    [ObservableProperty]
    private bool _isOpenBackgroundPanel;

    [ObservableProperty]
    private ExtendClientAreaChromeHints _systemChromeType;

    public MainWindowViewModel(HostNavigationService navigationService) {
        navigationService.NavigationRequest += p => {
            Dispatcher.Post(() => {
                ActivePage = null;
                ActivePage = p;
            }, DispatcherPriority.ApplicationIdle);
        };
        
        _navigationService = navigationService;
        _navigationService.NavigationTo<HomePageViewModel>();

        SystemChromeType = EnvironmentUtil.IsWindow ? ExtendClientAreaChromeHints.NoChrome : ExtendClientAreaChromeHints.Default;
    }


    [RelayCommand]
    private void NavigationTo(string pageKey) {
        IsOpenBackgroundPanel = pageKey switch {
            "HomePage" => false,
            "SettingNavigationPage" => true,
            "DownloadNavigationPage" => true,
            _ => false
        };
        
        switch (pageKey) {
            case "HomePage":
                _navigationService.NavigationTo<HomePageViewModel>(); 
                break;
            case "SettingNavigationPage":
                _navigationService.NavigationTo<SettingNavigationPageViewModel>(); 
                break;
            case "DownloadNavigationPage":
                _navigationService.NavigationTo<DownloadNavigationPageViewModel>(); 
                break;
            default:
                _navigationService.NavigationTo<HomePageViewModel>();
                break;
        }
    }
}