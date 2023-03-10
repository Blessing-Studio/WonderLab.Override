using Avalonia;
using Avalonia.Controls;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control.Controls.Dialog;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Windows
{
    public class MainWindowViewModel : ReactiveObject
    {
        public List<ModLoaderModel> forges, fabrics, quilt, optifine;

        public MainWindowViewModel() {
            this.PropertyChanged += OnPropertyChanged;

            GetGameCoresAction();
        }

        [Reactive]
        public UserControl CurrentPage { get; set; } = new HomePage();

        [Reactive]
        public string InstallerTitle { get; set; } = "选择一个 Minecraft 版本核心";

        [Reactive]
        public double DownloadProgress { get; set; } = 0.0;

        [Reactive]
        public double InstallerTitleOpacity { get; set; } = 1;

        [Reactive]
        public bool HasForge { get; set; } = false;

        [Reactive]
        public bool HasFabric { get; set; } = false;

        [Reactive]
        public bool HasQuilt { get; set; } = false;

        [Reactive]
        public bool HasOptifine { get; set; } = false;

        [Reactive]
        public bool ModLoaderVisible { get; set; } = false;

        [Reactive]
        public bool McVersionVisible { get; set; } = true;

        [Reactive]
        public GameCoreEmtity CurrentGameCore { get; set; }

        [Reactive]
        public ModLoaderViewData CurrentModLoader { get; set; }

        [Reactive]
        public ObservableCollection<GameCoreEmtity> GameCores { get; set; } = new();

        [Reactive]
        public ObservableCollection<ModLoaderViewData> ModLoaders { get; set; } = new();

        [Reactive]
        public ObservableCollection<ModLoaderViewData> CurrentModLoaders { get; set; } = new();

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }

            if (e.PropertyName == nameof(CurrentGameCore)) { 
                HideAllLoaderButton();
                CurrentModLoaders.Clear();

                await Task.Run(async () => { 
                    var forges = await Task.Run(async () => {
                        var result = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(CurrentGameCore.Id)).Select(x => new ModLoaderModel() {                       
                            ModLoaderType = ModLoaderType.Forge,
                            ModLoaderBuild = x,
                            GameCoreVersion = x.McVersion,
                            Id = x.ForgeVersion,
                            Time = x.ModifiedTime
                        });
                    
                        if (!result.Any()) {
                            HasForge = false;
                            return new List<ModLoaderModel>();
                        }

                        HasForge = true;
                        this.forges = result.ToList();
                        return result.ToList();
                    });
                    
                    var optifines = await Task.Run(async () => {
                        var result = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(CurrentGameCore.Id)).Select(x => new ModLoaderModel() { 
                            ModLoaderType = ModLoaderType.OptiFine,
                            ModLoaderBuild = x,
                            GameCoreVersion = x.McVersion,
                            Id = x.Type,
                            Time = DateTime.Now
                        });
                    
                        if (!result.Any()) {
                            HasOptifine = false;
                            return new List<ModLoaderModel>();
                        }

                        HasOptifine = true;
                        this.optifine = result.ToList();
                        return result.ToList();
                    });
                    
                    var fabrics = await Task.Run(async () => {
                        var result = (await FabricInstaller.GetFabricBuildsByVersionAsync(CurrentGameCore.Id)).Select(x => new ModLoaderModel() { 
                            ModLoaderType = ModLoaderType.Fabric,
                            GameCoreVersion = x.Intermediary.Version,
                            ModLoaderBuild = x,
                            Id = x.Loader.Version,
                            Time = DateTime.Now
                        });
                    
                        if (!result.Any()) {
                            HasFabric = false;
                            return new List<ModLoaderModel>();
                        }

                        HasFabric = true;
                        this.fabrics = result.ToList();
                        return result.ToList();
                    });
                    
                    var quilts = await Task.Run(async () => {
                        var result = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(CurrentGameCore.Id)).Select(x => new ModLoaderModel() { 
                            ModLoaderType = ModLoaderType.Quilt,
                            GameCoreVersion = x.Intermediary.Version,
                            ModLoaderBuild = x,
                            Id = x.Loader.Version,
                            Time = DateTime.Now
                        });
                    
                        if (!result.Any()) {
                            HasQuilt = false;
                            return new List<ModLoaderModel>();
                        }

                        HasQuilt = true;
                        this.quilt = result.ToList();
                        return result.ToList();
                    });
                });
            }

            if (e.PropertyName == nameof(CurrentModLoader) && CurrentModLoader is not null) {
                foreach (var i in CurrentModLoaders) {
                    if (i.Data.ModLoaderType == CurrentModLoader.Data.ModLoaderType) {
                        CurrentModLoader = null!;

                        ModLoaderVisible = false;
                        McVersionVisible = true;
                        "无法选择多个相同类型的加载器！".ShowMessage();
                        return;
                    }

                    if (i.Data.ModLoaderType == ModLoaderType.Forge && (CurrentModLoader.Data.ModLoaderType == ModLoaderType.Fabric ||
                        CurrentModLoader.Data.ModLoaderType == ModLoaderType.Quilt)) {
                        "Forge 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        ReturnMcListAction();
                        return;
                    }
                    else if (i.Data.ModLoaderType == ModLoaderType.OptiFine && (CurrentModLoader.Data.ModLoaderType == ModLoaderType.Fabric ||
                        CurrentModLoader.Data.ModLoaderType == ModLoaderType.Quilt)) {
                        "Optifine 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        ReturnMcListAction();
                        return;
                    }
                    else if (i.Data.ModLoaderType == ModLoaderType.Fabric && (CurrentModLoader.Data.ModLoaderType == ModLoaderType.Forge ||
                        CurrentModLoader.Data.ModLoaderType == ModLoaderType.Quilt ||
                        CurrentModLoader.Data.ModLoaderType == ModLoaderType.OptiFine)) {
                        "Fabric 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        ReturnMcListAction();
                        return;
                    }
                    else if (i.Data.ModLoaderType == ModLoaderType.Quilt && (CurrentModLoader.Data.ModLoaderType == ModLoaderType.Forge ||
                        CurrentModLoader.Data.ModLoaderType == ModLoaderType.Fabric ||
                        CurrentModLoader.Data.ModLoaderType == ModLoaderType.OptiFine)) {
                        "Quilt 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        ReturnMcListAction();
                        return;
                    }
                }

                ReturnMcListAction();
                CurrentModLoaders.Add(CurrentModLoader);
            }
        }

        public async void ChangeTitle(string title) {
            InstallerTitleOpacity = 0;
            await Task.Delay(360);
            InstallerTitle = title;
            await Task.Delay(200);
            InstallerTitleOpacity = 1;
        }

        public void HideAllLoaderButton() {
            HasOptifine = false;
            HasQuilt = false;
            HasFabric = false;
            HasForge = false;
        }

        public async void GetGameCoresAction() {
            var res = await GameCoreInstaller.GetGameCoresAsync();
            GameCores.Clear();
            
            var temp = res.Cores.Where(x => {
                x.Type = x.Type switch {
                    "snapshot" => "快照版本",
                    "release" => "正式版本",
                    "old_alpha" => "远古版本",
                    "old_beta" => "远古版本",
                    _ => "Fuck"
                } + $" {x.ReleaseTime.ToString(@"yyyy\-MM\-dd hh\:mm")}";
            
                return true;
            });
            
            foreach (var item in temp) {
                await Task.Delay(20);
                GameCores.Add(item);
            }
        }

        public void ReturnMcListAction() {
            ChangeTitle("选择一个 Minecraft 版本核心");
            ModLoaderVisible = false;
            McVersionVisible = true;            
        }

        public void HideInstallDialogAction() {
            MainWindow.Instance.InstallDialog.HideDialog();
        }

        public async void GameCoreInstallAction() {
            if (CurrentGameCore is null) {
                "无法进行安装，因为您还未选择任何游戏核心！".ShowMessage("提示");
                return;
            }

            if (CurrentModLoaders.Count <= 0) {
                NotificationViewData data = new();
                var installer = await Task.Run(() => new GameCoreInstaller("C:\\Users\\w\\Desktop\\temp\\.minecraft", CurrentGameCore.Id));
                data.Title = $"游戏 {CurrentGameCore.Id} 的安装任务";
                installer.ProgressChanged += (_, x) => {
                    data.ProgressOfBar = x.Progress * 100;
                    data.Progress = x.ProgressDescription!;
                };
                data.TimerStart();
                $"开始安装游戏 {CurrentGameCore.Id}！此过程不会很久，坐和放宽，您可以点击此条或下拉顶部条以查看下载进度！".ShowMessage(() => {
                    MainWindow.Instance.ShowTopBar();
                });

                MainWindow.Instance.InstallDialog.HideDialog();
                NotificationCenterPage.ViewModel.Notifications.Add(data);

                await Task.Delay(2000);
                var result = await Task.Run(async () => await installer.InstallAsync());
                if (result.Success) {
                    $"游戏 {CurrentGameCore.Id} 安装完成！".ShowMessage();
                    data.TimerStop();
                }

                return;
            }
        }
    }
}
