﻿using Avalonia.Threading;
using WonderLab.ViewModels.Pages;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.ViewModels.Pages.Navigation;
using WonderLab.Classes.Datas;
using System.Collections.ObjectModel;
using WonderLab.Services;
using WonderLab.Classes.Datas.TaskData;
using System;
using System.Timers;
using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Classes.Enums;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
    private readonly SettingService _settingService;
    private readonly DialogService _dialogService;
    private readonly TaskService _taskService;
    private readonly HostNavigationService _navigationService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private string _imagePath;

    [ObservableProperty] private int _blurRadius;

    [ObservableProperty] private object _activePage;
    [ObservableProperty] private ParallaxMode _parallaxMode;

    [ObservableProperty] private bool _isEnableBlur;
    [ObservableProperty] private bool _isOpenTaskListPanel;
    [ObservableProperty] private bool _isOpenBackgroundPanel;

    [ObservableProperty] private NavigationPageData _activePanelPage;
    [ObservableProperty] private ReadOnlyObservableCollection<ITaskJob> _tasks;
    [ObservableProperty] private ReadOnlyObservableCollection<INotification> _notifications;

    public MainWindowViewModel(
        TaskService taskService,
        DialogService dialogService,
        SettingService settingService,
        HostNavigationService navigationService,
        NotificationService notificationService) {
        _taskService = taskService;
        _dialogService = dialogService;
        _settingService = settingService;
        _navigationService = navigationService;
        _notificationService = notificationService;

        _navigationService.NavigationRequest += p => {
            if (p.PageKey is "HomePage") {
                ActivePage = p.Page;
            } else {
                ActivePanelPage = null;
                ActivePanelPage = p;
            }
        };

        WeakReferenceMessenger.Default.Register<BlurEnableMessage>(this, BlurEnableValueHandle);
        WeakReferenceMessenger.Default.Register<BlurRadiusChangeMessage>(this, BlurRadiusChangeHandle);
        WeakReferenceMessenger.Default.Register<ParallaxModeChangeMessage>(this, ParallaxModeChangeHandle);
    }

    [RelayCommand]
    private void ControlTaskListPanel() {
        IsOpenTaskListPanel = !IsOpenTaskListPanel;
    }

    [RelayCommand]
    private void NavigationTo(string pageKey) {
        IsOpenBackgroundPanel = pageKey switch {
            "HomePage" => false,
            "MultiplayerPage" => true,
            "SettingNavigationPage" => true,
            "DownloadNavigationPage" => true,
            _ => false
        };

        switch (pageKey) {
            case "HomePage":
                _navigationService.NavigationTo<HomePageViewModel>();
                break;
            case "MultiplayerPage":
                _navigationService.NavigationTo<MultiplayerPageViewModel>();
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

    private void BlurEnableValueHandle(object obj, BlurEnableMessage blurEnable) {
        IsEnableBlur = blurEnable.IsEnableBlur;
    }

    private void BlurRadiusChangeHandle(object obj, BlurRadiusChangeMessage blurRadiusChange) {
        BlurRadius = blurRadiusChange.BlurRadius;
    }

    private void ParallaxModeChangeHandle(object obj, ParallaxModeChangeMessage parallaxModeChange) {
        ParallaxMode = parallaxModeChange.ParallaxMode switch {
            0 => ParallaxMode.None,
            1 => ParallaxMode.Flat,
            2 => ParallaxMode.Solid,
            _ => ParallaxMode.None
        };
    }

    public void OnLoaded() {
        _taskService.QueueJob(new InitTask(_settingService, _dialogService, _notificationService));

        Tasks = new(_taskService.TaskJobs);
        Notifications = new(_notificationService.Notifications);

        ParallaxMode = _settingService.Data.ParallaxMode switch {
            0 => ParallaxMode.None,
            1 => ParallaxMode.Flat,
            2 => ParallaxMode.Solid,
            _ => ParallaxMode.None,
        };

        _navigationService.NavigationTo<HomePageViewModel>();
    }
}