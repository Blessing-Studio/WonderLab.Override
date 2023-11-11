using Avalonia.Controls;
using Avalonia.Threading;
using MinecraftLaunch.Events;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Install;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control.Animation;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class InstallerPageViewModel : ViewModelBase {
        private Control leftContent; 

        private Control rightContent;

        private bool Isswitch = false;

        private double progress = 0d;

        private string progressDescription = string.Empty;

        private CancellationTokenSource animationToken = new();

        private DispatcherTimer dispatcherTimer = new(DispatcherPriority.Normal);

        private readonly PageVaryAnimation varyAnimation = new(TimeSpan.FromMilliseconds(500)) {
            IsHorizontal = true,
        };

        public InstallerPageViewModel(Control left, Control right) {      
            leftContent = left;
            rightContent = right;
        }

        [Reactive]
        public bool IsForgeLoaded { get; set; }

        [Reactive]
        public bool IsNeoForgeLoaded { get; set; }

        [Reactive]
        public bool IsFabricLoaded { get; set; }

        [Reactive]
        public bool IsOptifineLoaded { get; set; }

        [Reactive]
        public bool IsQuiltLoaded { get; set; }

        [Reactive]
        public ObservableCollection<ModLoaderModel> CurrentLoaders { get; set; } = new();

        [Reactive]
        public ObservableCollection<ModLoaderModel> SelectLoaders { get; set; } = new();

        public async void GotoAction() {
            await Dispatcher.InvokeAsync(async () => {
                leftContent.Opacity = 0;
                leftContent.IsHitTestVisible = false;

                animationToken.Cancel();
                animationToken.Dispose();
                animationToken = new();

                rightContent.Opacity = 1;
                rightContent.IsHitTestVisible = true;

                await varyAnimation.Start(leftContent, rightContent, true,
                    animationToken.Token);
            }, DispatcherPriority.Send);
        }

        public async void GobackAction() {
            await Dispatcher.InvokeAsync(async () => {
                rightContent.Opacity = 0;
                rightContent.IsHitTestVisible = false;

                animationToken.Cancel();
                animationToken.Dispose();
                animationToken = new();

                leftContent.Opacity = 1;
                leftContent.IsHitTestVisible = true;

                await varyAnimation.Start(rightContent, leftContent, false,
                    animationToken.Token);
            }, DispatcherPriority.Send);
        }

        public async void InstallAction() {
            List<InstallerBase<InstallerResponse>> installers = new();
            var infoData = GlobalResources.LaunchInfoData;
            var gameUtil = infoData.GameDirectoryPath;
            if (SelectLoaders.Count > 2) {
                "选择了太多的加载器！".ShowMessage();
                return;
            }

            if (SelectLoaders.Count == 0) {
                await Task.Run(() => installers.Add(new GameCoreInstaller(gameUtil, CacheResources
                    .GameCoreInstallInfo.Id)));
            }

            foreach (var loader in SelectLoaders) {
                if (loader.ModLoaderType is ModLoaderType.Forge) {
                    installers.Add(new ForgeInstaller(gameUtil,loader.ModLoaderBuild as ForgeInstallEntity,
                        infoData.JavaRuntimePath.JavaPath));
                } else if (loader.ModLoaderType is ModLoaderType.NeoForged) {
                    installers.Add(new NeoForgeInstaller(gameUtil, loader.ModLoaderBuild as NeoForgeInstallEntity, 
                        infoData.JavaRuntimePath.JavaPath));
                } else if (loader.ModLoaderType is ModLoaderType.OptiFine) {
                    installers.Add(new OptiFineInstaller(gameUtil, loader.ModLoaderBuild as OptiFineInstallEntity,
                        infoData.JavaRuntimePath.JavaPath));
                }else if (loader.ModLoaderType is ModLoaderType.Quilt) {
                    installers.Add(new QuiltInstaller(gameUtil, loader.ModLoaderBuild as QuiltInstallBuild));
                } else if (loader.ModLoaderType is ModLoaderType.Fabric) {
                    installers.Add(new FabricInstaller(gameUtil, loader.ModLoaderBuild as FabricInstallBuild));
                }
            }

            StringBuilder customId = new(CacheResources
                .GameCoreInstallInfo.Id);

            foreach (var item in SelectLoaders) {
                customId.Append($"-{item.ModLoader}_{item.ModLoaderBuild.ToString()}");
            }

            $"开始安装游戏 {customId}！此过程不会很久，坐和放宽，您可以点击此条的跳转按钮进入通知中心以查看下载进度！"
                .ShowMessage(App.CurrentWindow.NotificationCenter.Open);

            NotificationViewData data = new(NotificationType.Install) { 
                Title = $"游戏 {customId} 的安装任务",
            };

            data.TimerStart();
            NotificationCenterPage.ViewModel.Notifications
                .Add(data);

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimer.Tick += (_, x) => {
                data.ProgressOfBar = progress;
                data.Progress = progressDescription;
            };

            dispatcherTimer.Start();
            foreach (var installer in installers) {
                await Task.Run(async () => {
                    installer.CustomId = customId.ToString();
                    installer.ProgressChanged += OnInstallProgressChanged;
                    var result = await installer.InstallAsync();
                }).WaitAsync(default(CancellationToken));
            }

            data.TimerStop();
            void OnInstallProgressChanged(object sender, ProgressChangedEventArgs e) {
                progress = e.Progress * 100;
                progressDescription = $"{progress:0.00}%";
            }
        }
    }
}
