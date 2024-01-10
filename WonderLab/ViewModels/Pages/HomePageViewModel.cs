using System;
using System.IO;
using System.Threading;
using Avalonia.Threading;
using WonderLab.Views.Pages;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Classes.Utilities;
using CommunityToolkit.Mvvm.Input;
using WonderLab.ViewModels.Windows;
using WonderLab.Classes.Attributes;
using WonderLab.Classes.Models.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using MinecraftLaunch.Classes.Models.Game;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Classes.Models.ViewData;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Classes.Extensions;
using WonderLab.Services;
using NotificationService = WonderLab.Services.UI.NotificationService;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly DataService _dataService;
    private readonly TaskService _taskService;
    private readonly ConfigDataModel _configData;
    private readonly GameEntryService _gameEntryService;
    private readonly NotificationService _notificationService;
    private readonly DispatcherTimer _timer = new();
        
    [ObservableProperty]
    private bool isOpenGameCoreBar;
        
    [ObservableProperty]
    private double otherControlOpacity = 1;

    [ObservableProperty]
    private double controlCenterBarWidth = 180;

    [ObservableProperty]
    private GameViewData? selectedGameCore;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LaunchGameCommand))]
    private GameViewData? selectedGameCoreInfo;

    [ObservableProperty]
    private ObservableCollection<GameViewData> gameCores = new();

    [ObservableProperty]
    private string nowTime = DateTime.Now.ToString("tt hh:mm");

    [ObservableProperty]
    [BindToConfig("CurrentGameCoreId")]
    private string? currentGameCoreId;

    public HomePageViewModel(
        DataService dataService,
        TaskService taskService,
        NotificationService notificationService,
        GameEntryService gameEntryService) {
        _taskService = taskService;
        _dataService = dataService;
        _configData = dataService.ConfigData;
        _gameEntryService = gameEntryService;
        _notificationService = notificationService;
            
        Init();
        WeakReferenceMessenger.Default.Register<GameViewData>(this, HandleMessage);
    }

    [RelayCommand]
    private async Task ControlControlCenterBar() {
        await Dispatcher.UIThread.InvokeAsync(() => {
            var homePage = App.ServiceProvider
                .GetRequiredService<HomePage>();

            var vm = App.ServiceProvider
                .GetRequiredService<MainWindowViewModel>();

            if (OtherControlOpacity is 0) {
                vm.IsFullScreen = false;
                ControlCenterBarWidth = 180;
                OtherControlOpacity = 1;
            } else {
                //OpenControlCenter(homePage);
            }
                
        });
    }

    [RelayCommand(CanExecute = nameof(CanLaunchGame))]
    private async Task LaunchGame() {
        await Dispatcher.UIThread.InvokeAsync(() => {
            _notificationService.Info($"尝试启动游戏 [{SelectedGameCoreInfo?.Data.Id}] 可在控制中心里的任务列表查看实时进度");
        });

        LaunchTask task = new(SelectedGameCoreInfo!.Data!, _dataService, _notificationService);
        _taskService.QueueJob(task);
    }

    [RelayCommand]
    private async Task GetGameCore() {
        if (!Directory.Exists(_configData.GameFolder)) {
            return;
        }
            
        GameCores.Clear();
        await Task.Run(() => _gameEntryService
            .GetGameEntries(_configData.GameFolder)
            .ToList()
            .CreateEnumerable<GameEntry, GameViewData>())
            .ContinueWith(async task => {
            await Dispatcher.UIThread.InvokeAsync(async () => {
                GameCores.Load(await task);
            });
        });
    }

    private async void Init() {
        _timer.Interval = TimeSpan.FromMinutes(1);
        _timer.Tick += (sender, args) => {
            NowTime = DateTime.Now.ToString("tt hh:mm");
        };
        _timer.Start();

        if (!Directory.Exists(_configData.GameFolder)) {
            return;
        }

        await Task.Run(() => {
            SelectedGameCoreInfo = _gameEntryService.GetGameEntry(_configData.GameFolder,
                _configData.CurrentGameCoreId).CreateViewData<GameEntry, GameViewData>();
        });
    }
        
    private bool CanLaunchGame() {
        return SelectedGameCoreInfo is not null;
    }
        
    private async void HandleMessage(object obj, GameViewData message) {
        await GameOperationBar.Scope.CollapseInterface();
        SelectedGameCore = message;
        SelectedGameCoreInfo = message;
    }
}