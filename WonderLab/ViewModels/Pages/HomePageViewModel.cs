using System;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Threading;
using System.Windows.Input;
using WonderLab.Views.Pages;
using System.ComponentModel;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using WonderLab.Classes.Managers;
using WonderLab.Classes.Utilities;
using WonderLab.ViewModels.Windows;
using WonderLab.Classes.Attributes;
using Avalonia.Controls.Converters;
using WonderLab.Classes.Models.Tasks;
using System.Collections.ObjectModel;
using MinecraftLaunch.Modules.Utilities;
using WonderLab.Views.Pages.ControlCenter;
using MinecraftLaunch.Modules.Models.Launch;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WonderLab.ViewModels.Pages {
    public partial class HomePageViewModel : ViewModelBase {
        private TaskManager _taskManager;

        private ConfigDataModel _configData;

        private NotificationManager _notificationManager;

        private DispatcherTimer _timer = new();

        [ObservableProperty]
        public bool isOpenGameCoreBar = false;

        [ObservableProperty]
        public double gameCoreBarHeight = 85;

        [ObservableProperty]
        public double gameCoreBarWidth = 155;

        [ObservableProperty]
        public double gameCoreListOpacity = 0;

        [ObservableProperty]
        public double otherControlOpacity = 1;

        [ObservableProperty]
        public double controlCenterBarWidth = 180;

        [ObservableProperty]
        public GameCore? selectedGameCore;

        [ObservableProperty]
        public GameCore? selectedGameCoreInfo;

        [ObservableProperty]
        public ObservableCollection<GameCore> gameCores = new();

        [ObservableProperty]
        public string nowTime = DateTime.Now.ToString("tt hh:mm");

        [ObservableProperty]
        [BindToConfig("CurrentGameCoreId")]
        public string? currentGameCoreId;

        public object TaskCenterCardContent => App.ServiceProvider
            .GetRequiredService<TaskCenterPage>();

        public object NotificationCenterCardContent => App.ServiceProvider
            .GetRequiredService<NotificationCenterPage>();

        public HomePageViewModel(DataManager dataManager, TaskManager taskManager,
            NotificationManager notificationManager) : base(dataManager) {
            _taskManager = taskManager;
            _configData = dataManager.Config;
            _notificationManager = notificationManager;
            Init();

            //this.WhenAnyValue(p1 => p1.SelectedGameCore)
            //    .Subscribe(core => {
            //        if(core != null) {
            //            SelectedGameCoreInfo = core;
            //            CurrentGameCoreId = core.Id!;
            //        }
            //    });
        }

        [RelayCommand]
        public async Task OpenGameCoreBar() {
            try {
                var homePage = App.ServiceProvider
                    .GetRequiredService<HomePage>();

                double height = homePage.Bounds.Height - 56,
                    width = homePage.Bounds.Width;

                GameCoreBarWidth = width;
                GameCoreBarHeight = height;
                IsOpenGameCoreBar = !IsOpenGameCoreBar;
                await Task.Delay(300).ContinueWith(async x => {
                    GameCoreListOpacity = 1;
                    await GetGameCore();
                });

                //homePage.WhenAnyValue(x => x.Bounds.Height, x => x.Bounds.Width)
                //.Subscribe(x => {
                //    if (IsOpenGameCoreBar) {
                //        GameCoreBarWidth = homePage.Bounds.Width;
                //        GameCoreBarHeight = homePage.Bounds.Height - 56;
                //    }
                //});
            }
            catch (Exception) {

            }
        }

        [RelayCommand]
        public void CloseGameCoreBar() {
            GameCoreListOpacity = 0;
            GameCoreBarHeight = 85;
            GameCoreBarWidth = 155;
            IsOpenGameCoreBar = !IsOpenGameCoreBar;
        }

        [RelayCommand]
        public async Task ControlControlCenterBar() {
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
                    OpenControlCenter(homePage);
                }

                //homePage.WhenAnyValue(x => x.Bounds.Height, x => x.Bounds.Width)
                //.Subscribe(x => {
                //    if (OtherControlOpacity == 0) {
                //        ControlCenterBarWidth = homePage.Bounds.Width;
                //    }
                //});
            });
        }

        [RelayCommand]
        public async Task LaunchGame() {
            await Dispatcher.UIThread.InvokeAsync(() => {
                _notificationManager.Info($"尝试启动游戏 [{SelectedGameCore?.Id}] 可在控制中心里的任务列表查看实时进度");
            });

            LaunchTask task = new(SelectedGameCoreInfo!, _configData, _notificationManager);
            _taskManager.QueueJob(task);
        }

        public async void Init() {
            _timer.Interval = TimeSpan.FromMinutes(1);
            _timer.Tick += (sender, args) => {
                NowTime = DateTime.Now.ToString("tt hh:mm");
            };
            _timer.Start();

            if (!Directory.Exists(_configData.GameFolder)) {
                return;
            }

            await Task.Run(() => {
                SelectedGameCoreInfo = GameCoreUtil.GetGameCore(_configData.GameFolder,
                    _configData.CurrentGameCoreId);
            });
        }

        private async Task GetGameCore() {
            if (!Directory.Exists(_configData.GameFolder)) {
                return;
            }

            GameCores.Clear();
            await Task.Run(() => {
                return GameCoreUtil.GetGameCores(_configData.GameFolder).ToList();
            }).ContinueWith(async task => {
                await Dispatcher.UIThread.InvokeAsync(async () => {
                    GameCores.Load(await task);
                }, DispatcherPriority.Render);
            });
        }

        private void OpenControlCenter(HomePage homePage) {
            var vm = App.ServiceProvider
                .GetRequiredService<MainWindowViewModel>();

            vm.IsFullScreen = true;
            OtherControlOpacity = 0;
            ControlCenterBarWidth = homePage.Bounds.Width;
        }
    }
}