using System;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Threading;
using WonderLab.Views.Pages;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Classes.Managers;
using WonderLab.Classes.Utilities;
using CommunityToolkit.Mvvm.Input;
using WonderLab.ViewModels.Windows;
using WonderLab.Classes.Attributes;
using WonderLab.Classes.Models.Tasks;
using System.Collections.ObjectModel;
using WonderLab.Views.Pages.ControlCenter;
using MinecraftLaunch.Classes.Models.Game;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Classes.Models.ViewData;
using WonderLab.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using System.Reflection.Metadata;
using WonderLab.Views.Controls;

namespace WonderLab.ViewModels.Pages {
    public partial class HomePageViewModel : ViewModelBase {
        private readonly DataManager _dataManager;
        private readonly TaskManager _taskManager;
        private readonly ConfigDataModel _configData;
        private readonly GameCoreManager _gameCoreManager;
        private readonly NotificationManager _notificationManager;
        
        private const int MAX_GAMEBAR_WIDTH = 645;
        private const int MAX_GAMEBAR_HEIGHT = 370;
        private readonly DispatcherTimer _timer = new();
        private CancellationTokenSource _cancellationTokenSource = new();
        
        [ObservableProperty]
        private bool isOpenGameCoreBar = false;
      
        [ObservableProperty]
        private double gameCoreBarHeight = 85;

        [ObservableProperty]
        private double gameCoreBarWidth = 155;

        [ObservableProperty]
        private double gameCoreListOpacity = 0;

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
            DataManager dataManager,
            TaskManager taskManager,
            NotificationManager notificationManager,
            GameCoreManager gameCoreManager) : base(dataManager) {
            _taskManager = taskManager;
            _dataManager = dataManager;
            _configData = dataManager.Config;
            _gameCoreManager = gameCoreManager;
            _notificationManager = notificationManager;
            
            Init();
            WeakReferenceMessenger.Default.Register<GameViewData>(this, HandleMessage);
        }

        [RelayCommand]
        private async Task OpenGameCoreBar() {
            try {
                using (_cancellationTokenSource) {
                    _cancellationTokenSource?.Cancel();
                    _cancellationTokenSource = new();
                }

                await Dispatcher.UIThread.InvokeAsync(() => {
                    GameCoreBarWidth = MAX_GAMEBAR_WIDTH;
                    GameCoreBarHeight = MAX_GAMEBAR_HEIGHT;
                    IsOpenGameCoreBar = !IsOpenGameCoreBar;
                }, DispatcherPriority.Render, _cancellationTokenSource.Token);

                await Task.Delay(300).ContinueWith(async x => {
                    GameCoreListOpacity = 1;
                    await GetGameCore();
                }, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException) { }
        }

        [RelayCommand]
        private async Task CloseGameCoreBar() {
            try {
                using (_cancellationTokenSource) {
                    _cancellationTokenSource?.Cancel();
                    _cancellationTokenSource = new();
                }

                await Dispatcher.UIThread.InvokeAsync(() => {
                    GameCoreListOpacity = 0;
                    GameCoreBarHeight = 85;
                    GameCoreBarWidth = 155;
                    IsOpenGameCoreBar = !IsOpenGameCoreBar;
                }, DispatcherPriority.Render, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException) { }
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
                _notificationManager.Info($"尝试启动游戏 [{SelectedGameCoreInfo?.Data.Id}] 可在控制中心里的任务列表查看实时进度");
            });

            LaunchTask task = new(SelectedGameCoreInfo!.Data!, _dataManager, _notificationManager);
            _taskManager.QueueJob(task);
        }

        [RelayCommand]
        private async Task GetGameCore() {
            if (!Directory.Exists(_configData.GameFolder)) {
                return;
            }

            GameCores.Clear();
            await Task.Run(() => {
                return _gameCoreManager.GetGameEntries(_configData.GameFolder)
                    .ToList()
                    .CreateEnumerable<GameEntry, GameViewData>();
            }).ContinueWith(async task => {
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
                SelectedGameCoreInfo = _gameCoreManager.GetGameEntry(_configData.GameFolder,
                    _configData.CurrentGameCoreId).CreateViewData<GameEntry, GameViewData>();
            });
        }
        
        private bool CanLaunchGame() {
            return SelectedGameCoreInfo is not null;
        }
        
        private async void HandleMessage(object obj, GameViewData message) {
            if (message is null) {
                return;
            }

            await GameOperationBar.Scope.CollapseInterface();
            SelectedGameCore = message;
            SelectedGameCoreInfo = message;
        }
    }
}