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

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
   private readonly LogService _logService;
   private readonly TaskService _taskService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private bool _isTitleBarVisible;
    [ObservableProperty] private bool _isOpenTaskListPanel;
    [ObservableProperty] private bool _isOpenBackgroundPanel;
    [ObservableProperty] private NavigationPageData _activePage;
    [ObservableProperty] private ReadOnlyObservableCollection<ITaskJob> _notifications;

    public MainWindowViewModel(HostNavigationService navigationService, TaskService taskService, LogService logService) {
        navigationService.NavigationRequest += p => {
            Dispatcher.Post(() => {
                if (ActivePage?.PageKey != p.PageKey) {
                    ActivePage = null;
                    ActivePage = p;
                }
            }, DispatcherPriority.ApplicationIdle);
        };

        _logService = logService;
        _taskService = taskService;
        _navigationService = navigationService;
        Notifications = new(_taskService.TaskJobs);

        _navigationService.NavigationTo<HomePageViewModel>();
        IsTitleBarVisible = EnvironmentUtil.IsWindow ? true : false;
    }

    [RelayCommand]
    private void ControlTaskListPanel() {
        IsOpenTaskListPanel = IsOpenTaskListPanel ? false : true;
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

    [RelayCommand]
    private void AddTask() {
        _taskService.TaskJobs.Add(new LaunchTask());
        _logService.Debug(nameof(MainWindowViewModel), $"Current count is {Notifications.Count}");
    }

    [RelayCommand]
    private void RemoveTask() {
        _taskService.TaskJobs.Remove(Notifications.FirstOrDefault());
    }
}