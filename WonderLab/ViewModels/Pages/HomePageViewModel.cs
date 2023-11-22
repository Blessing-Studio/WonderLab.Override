using System;
using ReactiveUI;
using System.Linq;
using Avalonia.Threading;
using System.Windows.Input;
using WonderLab.Views.Pages;
using System.ComponentModel;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using WonderLab.Classes.Models;
using WonderLab.Classes.Managers;
using WonderLab.Classes.Utilities;
using WonderLab.ViewModels.Windows;
using WonderLab.Classes.Models.Tasks;
using System.Collections.ObjectModel;
using MinecraftLaunch.Modules.Utilities;
using WonderLab.Views.Pages.ControlCenter;
using MinecraftLaunch.Modules.Models.Launch;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Classes.Attributes;
using System.Threading;

namespace WonderLab.ViewModels.Pages {
    public class HomePageViewModel : ViewModelBase {
        private TaskManager _taskManager;

        private ConfigDataModel _configData;

        private NotificationManager _notificationManager;

        [Reactive]
        public bool IsOpenGameCoreBar { get; set; } = false;

        [Reactive]
        public double GameCoreBarHeight { get; set; } = 85;

        [Reactive]
        public double GameCoreBarWidth { get; set; } = 155;

        [Reactive]
        public double GameCoreListOpacity { get; set; } = 0;

        [Reactive]
        public double OtherControlOpacity { get; set; } = 1;

        [Reactive]
        public double ControlCenterBarWidth { get; set; } = 175;

        [Reactive]
        public GameCore? SelectedGameCore { get; set; }

        [Reactive]
        public GameCore? SelectedGameCoreInfo { get; set; }

        [Reactive]
        public ObservableCollection<GameCore> GameCores { get; set; } = new();

        [Reactive]
        [BindToConfig("CurrentGameCoreId")]
        public string? CurrentGameCoreId { get; set; }

        public object TaskCenterCardContent => App.ServiceProvider
            .GetRequiredService<TaskCenterPage>();

        public object NotificationCenterCardContent => App.ServiceProvider
            .GetRequiredService<NotificationCenterPage>();

        public ICommand LaunchGameCommand
            => ReactiveCommand.Create(LaunchGame);

        public ICommand OpenGameCoreBarCommand 
            => ReactiveCommand.Create(OpenGameCoreBar);

        public ICommand CloseGameCoreBarCommand
            => ReactiveCommand.Create(CloseGameCoreBar);

        public ICommand OpenControlCenterCommand
            => ReactiveCommand.Create(ControlControlCenterBar);

        public HomePageViewModel(DataManager dataManager, TaskManager taskManager,
            NotificationManager notificationManager) : base(dataManager) {
            _taskManager = taskManager;
            _configData = dataManager.Config;
            _notificationManager = notificationManager;
            Init();

            this.WhenAnyValue(p1 => p1.SelectedGameCore)
                .Subscribe(core => {
                    if(core != null) {
                        SelectedGameCoreInfo = core;
                        CurrentGameCoreId = core.Id!;
                    }
                });
        }

        public async void OpenGameCoreBar() {
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

                homePage.WhenAnyValue(x => x.Bounds.Height, x => x.Bounds.Width)
                .Subscribe(x => {
                    if (IsOpenGameCoreBar) {
                        GameCoreBarWidth = homePage.Bounds.Width;
                        GameCoreBarHeight = homePage.Bounds.Height - 56;
                    }
                });
            }
            catch (Exception) {

            }
        }

        public void CloseGameCoreBar() {
            GameCoreListOpacity = 0;
            GameCoreBarHeight = 85;
            GameCoreBarWidth = 155;
            IsOpenGameCoreBar = !IsOpenGameCoreBar;
        }

        public async void ControlControlCenterBar() {
            await Dispatcher.UIThread.InvokeAsync(() => {
                var homePage = App.ServiceProvider
                    .GetRequiredService<HomePage>();

                var vm = App.ServiceProvider
                    .GetRequiredService<MainWindowViewModel>();

                if (OtherControlOpacity is 0) {
                    vm.IsFullScreen = false;
                    ControlCenterBarWidth = 175;
                    OtherControlOpacity = 1;
                } else {
                    OpenControlCenter(homePage);
                }

                homePage.WhenAnyValue(x => x.Bounds.Height, x => x.Bounds.Width)
                .Subscribe(x => {
                    if (OtherControlOpacity == 0) {
                        ControlCenterBarWidth = homePage.Bounds.Width;
                    }
                });
            });
        }

        public async void LaunchGame() {
            await Dispatcher.UIThread.InvokeAsync(() => {
                _notificationManager.Info($"尝试启动游戏 [{SelectedGameCore?.Id}] 可在控制中心里的任务列表查看实时进度");
            });

            LaunchTask task = new(SelectedGameCoreInfo!, _configData, _notificationManager);
            _taskManager.QueueJob(task);
        }

        private void Init() {
            var cores = GameCoreUtil.GetGameCores(_configData.GameFolder)
                .ToList();

            SelectedGameCoreInfo = cores
                .FirstOrDefault(x => x.Id == CurrentGameCoreId);
        }

        private async Task GetGameCore() {
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
