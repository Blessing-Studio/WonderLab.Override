using Avalonia.Platform;
using Avalonia.Threading;
using MinecraftLaunch.Utilities;
using WonderLab.ViewModels.Pages;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.ViewModels.Pages.Navigation;
using WonderLab.Classes.Datas;
using System.Collections.ObjectModel;
using WonderLab.Classes.Datas.ViewData;
using System.Linq;
using WonderLab.Services;
using WonderLab.Classes.Datas.TaskData;
using System;
using System.Timers;
using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Classes.Enums;
using System.Diagnostics;
using Avalonia;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
    private readonly TaskService _taskService;
    private readonly INavigationService _navigationService;
    private readonly NotificationService _notificationService;
    private readonly Timer _timer = new(TimeSpan.FromSeconds(1));

    [ObservableProperty] private string _time;
    [ObservableProperty] private string _year;
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
        _navigationService = navigationService;
        _notificationService = notificationService;

        _navigationService.NavigationRequest += async p => {
            await Dispatcher.InvokeAsync(() => {
                if (ActivePanelPage?.PageKey != p.PageKey) {
                    if (p.PageKey is "HomePage") {
                        ActivePage = p.Page;
                    } else {
                        ActivePanelPage = null;
                        ActivePanelPage = p;
                    }
                }
            }, DispatcherPriority.ApplicationIdle);
        };

        _taskService.QueueJob(new InitTask(settingService, dialogService, notificationService));

        Time = DateTime.Now.ToString("t");
        Year = DateTime.Now.ToString("d");

        _timer.Elapsed += (_, args) => {
            Time = args.SignalTime.ToString("t");
            Year = args.SignalTime.ToString("d");
        };

        Tasks = new(_taskService.TaskJobs);
        Notifications = new(_notificationService.Notifications);

        ParallaxMode = settingService.Data.ParallaxMode switch {
            0 => ParallaxMode.None,
            1 => ParallaxMode.Flat,
            2 => ParallaxMode.Solid,
            _ => ParallaxMode.None,
        };

        _navigationService.NavigationTo<HomePageViewModel>();
        _timer.Start();

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
}