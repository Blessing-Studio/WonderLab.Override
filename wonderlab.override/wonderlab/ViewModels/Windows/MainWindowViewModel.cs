using Avalonia;
using Avalonia.Controls;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
                var currentmodLoaderType = CurrentModLoader.Data.ModLoaderType;

                foreach (var i in CurrentModLoaders) {
                    if (i.Data.ModLoaderType == currentmodLoaderType) {
                        CurrentModLoader = null!;

                        ModLoaderVisible = false;
                        McVersionVisible = true;
                        "无法选择多个相同类型的加载器！".ShowMessage();
                        return;
                    }

                    if (i.Data.ModLoaderType == ModLoaderType.Forge && (currentmodLoaderType == ModLoaderType.Fabric ||
                        currentmodLoaderType == ModLoaderType.Quilt)) {
                        "Forge 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        ReturnMcListAction();
                        return;
                    }
                    else if (i.Data.ModLoaderType == ModLoaderType.OptiFine && (currentmodLoaderType == ModLoaderType.Fabric ||
                        currentmodLoaderType == ModLoaderType.Quilt)) {
                        "Optifine 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        ReturnMcListAction();
                        return;
                    }
                    else if (i.Data.ModLoaderType == ModLoaderType.Fabric && (currentmodLoaderType == ModLoaderType.Forge ||
                        currentmodLoaderType == ModLoaderType.Quilt || currentmodLoaderType == ModLoaderType.OptiFine))
                        {
                        "Fabric 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        ReturnMcListAction();
                        return;
                    }
                    else if (i.Data.ModLoaderType == ModLoaderType.Quilt && (currentmodLoaderType == ModLoaderType.Forge ||
                        currentmodLoaderType == ModLoaderType.Fabric || currentmodLoaderType == ModLoaderType.OptiFine)) {                       
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
            ModLoaderType currentmodloaderType = CurrentModLoaders.Any() ? CurrentModLoaders.First().Data.ModLoaderType : ModLoaderType.Any;
            NotificationViewData data = new();
            InstallerBase<InstallerResponse> installer = null;
            string customId = string.Empty;

            if (!CurrentModLoaders.Any()) {
                installer = await Task.Run(() => new GameCoreInstaller(App.LaunchInfoData.GameDirectoryPath, CurrentGameCore.Id));
                customId = CurrentGameCore.Id;
            }
            else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.Forge) {
                if (!App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw().IsFile()) {
                    "无法继续安装，因为未选择任何 Java！".ShowMessage();
                    return;
                }

                var installerdata = CurrentModLoaders.First().Data;
                customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                installer = await Task.Run(() => new ForgeInstaller(App.LaunchInfoData.GameDirectoryPath,
                    installerdata.ModLoaderBuild as ForgeInstallEntity, App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(),
                    customId));
            }
            else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.Fabric) {
                var installerdata = CurrentModLoaders.First().Data;
                customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                installer = await Task.Run(() => new FabricInstaller(App.LaunchInfoData.GameDirectoryPath, installerdata.ModLoaderBuild as FabricInstallBuild, customId));
            }
            else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.Quilt) {
                var installerdata = CurrentModLoaders.First().Data;
                customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                installer = await Task.Run(() => new QuiltInstaller(App.LaunchInfoData.GameDirectoryPath, installerdata.ModLoaderBuild as QuiltInstallBuild, customId));
            }
            else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.OptiFine) {           
                if (!App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw().IsFile()) {               
                    "无法继续安装，因为未选择任何 Java！".ShowMessage();
                    return;
                }

                var installerdata = CurrentModLoaders.First().Data;
                customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                installer = await Task.Run(() => new OptiFineInstaller(App.LaunchInfoData.GameDirectoryPath,
                    installerdata.ModLoaderBuild as OptiFineInstallEntity, App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(),
                    customId: customId));
            }
            else if (CurrentModLoaders.Count == 2) {
                CompositeGameCoreAction(data);
                return;
            }

            MainWindow.Instance.InstallDialog.HideDialog();
            data.Title = $"游戏 {customId} 的安装任务";           
            $"开始安装游戏 {customId}！此过程不会很久，坐和放宽，您可以点击此条或下拉顶部条以查看下载进度！".ShowMessage(() => {
                MainWindow.Instance.ShowTopBar();
            });
            NotificationCenterPage.ViewModel.Notifications.Add(data);
            await Task.Delay(2000);
            data.TimerStart();
            installer.ProgressChanged += async (_, x) => {
                var progress = x.Progress * 100;
                data.ProgressOfBar = progress;
                data.Progress = $"{Math.Round(progress, 2)}%";
            };

            var result = await Task.Run(async () => await installer!.InstallAsync());
            if (result.Success) {           
                $"游戏 {customId} 安装完成！".ShowMessage();
                data.TimerStop();
            }
        }

        public async void CompositeGameCoreAction(NotificationViewData data) {
            if (!App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw().IsFile()) {           
                "无法继续安装，因为未选择任何 Java！".ShowMessage();
            }

            MainWindow.Instance.InstallDialog.HideDialog();
            NotificationCenterPage.ViewModel.Notifications.Add(data);
            var forgedata = CurrentModLoaders.GetForge().Data;
            var optifinrdata = CurrentModLoaders.GetOptiFine().Data;
            var customId = $"{forgedata.GameCoreVersion}-{forgedata.ModLoader.ToLower()}-{forgedata.Id}";
            data.Title = $"游戏 {customId} 的安装任务";

            $"开始安装游戏 {customId}！此过程不会很久，坐和放宽，您可以点击此条或下拉顶部条以查看下载进度！".ShowMessage(() => {
                MainWindow.Instance.ShowTopBar();
            });

            data.Title = customId;
            data.TimerStart();
            ForgeInstaller installer = new(App.LaunchInfoData.GameDirectoryPath, forgedata.ModLoaderBuild as ForgeInstallEntity,
               App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(), customId);

            installer.ProgressChanged += ProcessOutPut;
            var result = await Task.Run(async () => await installer!.InstallAsync());

            if(result.Success) {
                OptiFineInstaller optiFineInstaller = new(App.LaunchInfoData.GameDirectoryPath, optifinrdata.ModLoaderBuild as OptiFineInstallEntity,
               App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(), customId: customId);
                optiFineInstaller.ProgressChanged += ProcessOutPut;
                var Optifineresult = await Task.Run(async () => await optiFineInstaller!.InstallAsync());
                if (Optifineresult.Success) {
                    $"游戏 {customId} 安装完成！".ShowMessage();
                    data.TimerStop();
                }
            }

            void ProcessOutPut(object? sender, MinecraftLaunch.Modules.Interface.ProgressChangedEventArgs x) {
                var progress = x.Progress * 100;
                data.ProgressOfBar = progress;
                data.Progress = $"{Math.Round(progress, 2)}%";
            }
        }
    }
}
