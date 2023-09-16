using MinecraftLaunch.Events;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Dialogs {
    public class GameInstallDialogViewModel : ReactiveObject {
        public string gamecoreId = string.Empty;

        public List<ModLoaderModel>? forges, fabrics, quilt, optifine;

        public GameInstallDialogViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        private async void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentGameCore)) {
                InstallerTitle = $"安装 - {CurrentGameCore.Id}";

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
                        "无法选择多个相同类型的加载器！".ShowMessage();
                        BackInstallerSelectAction();
                        return;
                    }

                    if (i.Data.ModLoaderType == ModLoaderType.Forge && (currentmodLoaderType == ModLoaderType.Fabric ||
                        currentmodLoaderType == ModLoaderType.Quilt)) {
                        "Forge 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        BackInstallerSelectAction();
                        return;
                    } else if (i.Data.ModLoaderType == ModLoaderType.OptiFine && (currentmodLoaderType == ModLoaderType.Fabric ||
                          currentmodLoaderType == ModLoaderType.Quilt)) {
                        "Optifine 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        BackInstallerSelectAction();
                        return;
                    } else if (i.Data.ModLoaderType == ModLoaderType.Fabric && (currentmodLoaderType == ModLoaderType.Forge ||
                          currentmodLoaderType == ModLoaderType.Quilt || currentmodLoaderType == ModLoaderType.OptiFine)) {
                        "Fabric 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        BackInstallerSelectAction();
                        return;
                    } else if (i.Data.ModLoaderType == ModLoaderType.Quilt && (currentmodLoaderType == ModLoaderType.Forge ||
                          currentmodLoaderType == ModLoaderType.Fabric || currentmodLoaderType == ModLoaderType.OptiFine)) {
                        "Quilt 无法与此加载器同时安装，WonderLab已自动将其移除安装队列！".ShowMessage("提示");
                        BackInstallerSelectAction();
                        return;
                    }
                }

                CurrentModLoaders.Add(CurrentModLoader);
                BackInstallerSelectAction();
            }
        }

        [Reactive]
        public string InstallerTitle { get; set; } = "安装 - *";

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
        public bool IsInstallerVisible { get; set; } = true;

        [Reactive]
        public GameCoreEmtity CurrentGameCore { get; set; } = null!;

        [Reactive]
        public ModLoaderViewData CurrentModLoader { get; set; } = null!;

        [Reactive]
        public ObservableCollection<ModLoaderViewData> ModLoaders { get; set; } = new();

        [Reactive]
        public ObservableCollection<ModLoaderViewData> CurrentModLoaders { get; set; } = new();

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

        public void HideInstallDialogAction() {
            //App.CurrentWindow.dialogHost.Install.InstallDialog.HideDialog();
        }

        public async void GameCoreInstallAction() {
            NotificationViewData data = new(NotificationType.Install);

            try {
                if (CurrentGameCore is null) {
                    "无法进行安装，因为您还未选择任何游戏核心！".ShowMessage("提示");
                    return;
                }

                ModLoaderType currentmodloaderType = CurrentModLoaders.Any() ? CurrentModLoaders.First().Data.ModLoaderType : ModLoaderType.Any;
                InstallerBase<InstallerResponse>? installer = null;
                string customId = string.Empty;

                if (!CurrentModLoaders.Any()) {
                    installer = await Task.Run(() => new GameCoreInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath, CurrentGameCore.Id));
                    customId = CurrentGameCore.Id;
                } else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.Forge) {
                    if (GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.IsNull() && !GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw().IsFile()) {
                        "无法继续安装，因为未选择任何 Java！".ShowMessage();
                        return;
                    }

                    var installerdata = CurrentModLoaders.First().Data;
                    customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                    installer = await Task.Run(() => new ForgeInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath,
                        (installerdata.ModLoaderBuild as ForgeInstallEntity)!, GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(),
                        customId));
                } else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.Fabric) {
                    var installerdata = CurrentModLoaders.First().Data;
                    customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                    installer = await Task.Run(() => new FabricInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath, (installerdata.ModLoaderBuild as FabricInstallBuild)!, customId));
                } else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.Quilt) {
                    var installerdata = CurrentModLoaders.First().Data;
                    customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                    installer = await Task.Run(() => new QuiltInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath, (installerdata.ModLoaderBuild as QuiltInstallBuild)!, customId));
                } else if (CurrentModLoaders.Count == 1 && currentmodloaderType is ModLoaderType.OptiFine) {
                    if (!GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw().IsFile()) {
                        "无法继续安装，因为未选择任何 Java！".ShowMessage();
                        return;
                    }

                    var installerdata = CurrentModLoaders.First().Data;
                    customId = $"{installerdata.GameCoreVersion}-{installerdata.ModLoader.ToLower()}-{installerdata.Id}";
                    installer = await Task.Run(() => new OptiFineInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath,
                        (installerdata.ModLoaderBuild as OptiFineInstallEntity)!, GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(),
                        customId: customId));
                } else if (CurrentModLoaders.Count == 2) {
                    CompositeGameCoreAction(data);
                    return;
                }

                data.Title = $"游戏 {customId} 的安装任务";
                $"开始安装游戏 {customId}！此过程不会很久，坐和放宽，您可以点击此条进入通知中心以查看下载进度！".ShowMessage(App.CurrentWindow.NotificationCenter.Open);
                NotificationCenterPage.ViewModel.Notifications.Add(data);
                await Task.Delay(2000);
                data.TimerStart();

                installer!.ProgressChanged += ProcessOutPut;

                var result = await Task.Run(async () => await installer!.InstallAsync());
                if (result.Success) {
                    $"游戏 {customId} 安装完成！".ShowMessage();
                    data.TimerStop();
                }
            }
            catch (Exception ex) {
                $"WonderLab 在安装游戏核心时遭遇了不可描述的错误,详细信息：{ex.Message}".ShowMessage("我日，炸了");
            }

            async void ProcessOutPut(object? sender, ProgressChangedEventArgs x) {
                try {
                    x.ProgressDescription.ShowLog();
                    await Task.Run(async() => {
                        var progress = x.Progress * 100;
                        data.ProgressOfBar = progress;
                        data.Progress = $"{Math.Round(progress, 2)}%";

                        await Task.Delay(1000);
                    });
                }
                catch (Exception ex) {
                    ex.ShowLog();
                }
            }
        }

        public void BackInstallerSelectAction() {
            IsInstallerVisible = true;
            ModLoaderVisible = false;
            ChangeTitle($"安装 - {CurrentGameCore.Id}");
        }

        public async void CompositeGameCoreAction(NotificationViewData data) {
            if (!GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw().IsFile()) {
                "无法继续安装，因为未选择任何 Java！".ShowMessage();
            }

            NotificationCenterPage.ViewModel.Notifications.Add(data);
            var forgedata = CurrentModLoaders.GetForge().Data;
            var optifinrdata = CurrentModLoaders.GetOptiFine().Data;
            var customId = $"{forgedata.GameCoreVersion}-{forgedata.ModLoader.ToLower()}-{forgedata.Id}";
            data.Title = $"游戏 {customId} 的安装任务";

            $"开始安装游戏 {customId}！此过程不会很久，坐和放宽，您可以点击此条进入通知中心以查看下载进度！".ShowMessage(() => {
                App.CurrentWindow.NotificationCenter.Open();
            });

            data.Title = customId;
            data.TimerStart();
            ForgeInstaller installer = new(GlobalResources.LaunchInfoData.GameDirectoryPath, (forgedata.ModLoaderBuild as ForgeInstallEntity)!,
               GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(), customId);

            installer.ProgressChanged += ProcessOutPut;
            var result = await Task.Run(async () => await installer!.InstallAsync());

            if (result.Success) {
                OptiFineInstaller optiFineInstaller = new(GlobalResources.LaunchInfoData.GameDirectoryPath, (optifinrdata.ModLoaderBuild as OptiFineInstallEntity)!,
               GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw(), customId: customId);
                optiFineInstaller.ProgressChanged += ProcessOutPut;
                var Optifineresult = await Task.Run(async () => await optiFineInstaller!.InstallAsync());
                if (Optifineresult.Success) {
                    $"游戏 {customId} 安装完成！".ShowMessage();
                    data.TimerStop();
                }
            }

            async void ProcessOutPut(object? sender, ProgressChangedEventArgs x) {
                try {
                    var progress = x.Progress * 100;
                    data.ProgressOfBar = progress;
                    data.Progress = $"{Math.Round(progress, 2)}%";

                    await Task.Delay(1000);
                }
                catch (Exception ex) {
                    ex.ShowLog();
                }
            }
        }
    }
}
