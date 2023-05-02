using Microsoft.CodeAnalysis;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using wonderlab.Class.Enum;
using wonderlab.Class.Models;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;

namespace wonderlab.Class.Utils
{
    public static class ModpacksUtils {
        /// <summary>
        /// 获取整合包类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ModpacksType ModpacksTypeAnalysis(string path) {     
            using var zipItems = ZipFile.OpenRead(path);

            foreach (var zipItem in zipItems.Entries) {
                if (zipItem.FullName.Contains("mcbbs.packmeta")) {
                    return ModpacksType.Mcbbs;
                }

                if (zipItem.FullName.Contains("modrinth.index.json")) {
                    return ModpacksType.Modrinth;
                }
            }

            return ModpacksType.Curseforge;
        }

        public static async ValueTask ModpacksInstallAsync(string path) {
            var type = ModpacksTypeAnalysis(path);

            if(type == ModpacksType.Mcbbs) {
                await McbbsModpacksInstallAsync(path);
            }
            else if (type == ModpacksType.Curseforge) {
                await CurseforgeModpacksInstallAsync(path);
            }
            else if (type == ModpacksType.Modrinth) {
                await ModrinthModpacksInstallAsync(path);
            }
        }

        public static async ValueTask McbbsModpacksInstallAsync(string path) {
            //action(0.1f, "开始获取整合包信息");
            string json = string.Empty;
            using ZipArchive zipinfo = ZipFile.OpenRead(path);
            if (zipinfo.GetEntry("manifest.json") != null) {           
                json = ZipExtension.GetString(zipinfo.GetEntry("manifest.json"));
            }

            var modpackInfo = json.ToJsonEntity<ModsPacksModel>();
            modpackInfo.Name.ShowLog();

            $"开始安装整合包 {modpackInfo.Name}！此过程不会很久，坐和放宽，您可以点击此条进入通知中心以查看下载进度！".ShowMessage(() => {
                MainWindow.Instance.NotificationCenter.Open();
            });

            NotificationViewData data = new() {           
                Title = $"整合包 {modpackInfo.Name} 的安装任务",
            };
            data.TimerStart();
            NotificationCenterPage.ViewModel.Notifications.Add(data);

            //游戏核心安装
            if (GameCoreToolkit.GetGameCore(App.LaunchInfoData.GameDirectoryPath, modpackInfo.Name) == null) {           
                await GameCoreUtils.CompLexGameCoreInstallAsync(modpackInfo.Minecraft.Version, modpackInfo.Name, async (x, e) => {
                    x.ShowLog();

                    data.ProgressOfBar = e;
                    data.Progress = $"{Math.Round(e, 2)}%";
                    await Task.Delay(1000);
                }, modpackInfo.Minecraft.ModLoaders);
            }

            data.ProgressOfBar = 0;
            data.Progress = $"0%";
            await Task.Delay(1000);

            var gamcorePath = GameCoreToolkit.GetGameCore(App.LaunchInfoData.GameDirectoryPath, modpackInfo.Name).GetGameCorePath();
            await Task.Run(() => {           
                using (ZipArchive subPath = ZipFile.OpenRead(path)) {               
                    foreach (ZipArchiveEntry i in subPath.Entries.AsParallel()) {                   
                        if (i.FullName.StartsWith("overrides") && !string.IsNullOrEmpty(ZipExtension.GetString(subPath.GetEntry(i.FullName)))) {                       
                            string cutpath = i.FullName.Replace("overrides/", string.Empty);
                            FileInfo v = new FileInfo(Path.Combine(gamcorePath, cutpath));
                            if (!Directory.Exists(Path.Combine(gamcorePath, v.Directory.FullName))) {                           
                                Directory.CreateDirectory(Path.Combine(gamcorePath, v.Directory.FullName));
                            }
                            ZipExtension.ExtractTo(subPath.GetEntry(i.FullName), Path.Combine(gamcorePath, cutpath));
                        }
                    }
                }
            });

            data.TimerStop();
            $"整合包 {modpackInfo.Name} 安装成功！".ShowMessage("成功");
        }

        public static async ValueTask CurseforgeModpacksInstallAsync(string path) {
            ModsPacksInstaller installer = new(path, App.LaunchInfoData.GameDirectoryPath);
            var info = await installer.GetModsPacksInfoAsync();
            $"开始安装整合包 {info.Name}！此过程不会很久，坐和放宽，您可以点击此条进入通知中心以查看下载进度！".ShowMessage(() => {
                MainWindow.Instance.NotificationCenter.Open();
            });
            
            NotificationViewData data = new() {
                Title = $"整合包 {info.Name} 的安装任务",                
            };
            data.TimerStart();
            NotificationCenterPage.ViewModel.Notifications.Add(data);

            //游戏核心安装
            if (GameCoreToolkit.GetGameCore(App.LaunchInfoData.GameDirectoryPath, info.Name) == null) {            
                await GameCoreUtils.CompLexGameCoreInstallAsync(info.Minecraft.Version, info.Name, async (x, e) => {
                    x.ShowLog();

                    data.ProgressOfBar = e;
                    data.Progress = $"{Math.Round(e, 2)}%";
                    await Task.Delay(1000);
                }, info.Minecraft.ModLoaders);
            }

            data.ProgressOfBar = 0;
            data.Progress = $"0%";
            await Task.Delay(1000);

            //资源安装
            installer.ProgressChanged += async (_, x) => {
                var progress = x.Progress * 100;
                data.ProgressOfBar = progress;
                data.Progress = $"{Math.Round(progress, 2)}%";

                x.ProgressDescription!.ShowLog();
                await Task.Delay(1000);
            };


            var result = await installer.InstallAsync();
            if (result.Success) {
                data.TimerStop();
                $"整合包 {info.Name} 安装成功！".ShowMessage("成功");
            }
        }

        public static async ValueTask ModrinthModpacksInstallAsync(string path) {
            string json = string.Empty;

            float _totalDownloaded = 0, _needToDownload = 0;
            using ZipArchive zipinfo = ZipFile.OpenRead(path);
            if (zipinfo.GetEntry("modrinth.index.json") != null) {           
                json = ZipExtension.GetString(zipinfo.GetEntry("modrinth.index.json"));
            }

            var modpackInfo = json.ToJsonEntity<ModrinthJsonModel>();
            modpackInfo.Name.ShowLog();
            _needToDownload = modpackInfo.Files.Count();

            $"开始安装整合包 {modpackInfo.Name}！此过程不会很久，坐和放宽，您可以点击此条进入通知中心以查看下载进度！".ShowMessage(() => {
                MainWindow.Instance.NotificationCenter.Open();
            });

            NotificationViewData data = new() {           
                Title = $"整合包 {modpackInfo.Name} 的安装任务",
            };
            data.TimerStart();
            NotificationCenterPage.ViewModel.Notifications.Add(data);

            //游戏核心安装
            await GameCoreUtils.CompLexGameCoreInstallAsync(modpackInfo.Name, async (x, e) => {           
                x.ShowLog();

                data.ProgressOfBar = e;
                data.Progress = $"{Math.Round(e, 2)}%";
                await Task.Delay(1000);
            }, modpackInfo.Dependencies);

            data.ProgressOfBar = 0;
            data.Progress = $"0%";
            await Task.Delay(1000);

            var gamecorePath = GameCoreToolkit.GetGameCore(App.LaunchInfoData.GameDirectoryPath, modpackInfo.Name).GetGameCorePath();
            //资源安装 -1
            var actionBlock = new ActionBlock<IEnumerable<Files>>(async x => {
                try {
                    foreach (var item in x.AsParallel()) {
                        foreach (var url in item.Downloads.AsParallel()) {
                            var folder = item.Path.Split('/').First();

                            var result = await HttpToolkit.HttpDownloadAsync(url, Path.Combine(gamecorePath, folder), Path.GetFileName(url));
                        }

                        _totalDownloaded++;
                        var e2 = _totalDownloaded / _needToDownload;

                        var progress = (e2 * 0.8f) * 100;
                        data.ProgressOfBar = progress;
                        data.Progress = $"{Math.Round(progress, 2)}%";
                        $"下载模组中：{_totalDownloaded}/{_needToDownload}".ShowLog();
                    }
                }
                catch (Exception) {               

                }
            },new ExecutionDataflowBlockOptions {
                BoundedCapacity = 64,
                MaxDegreeOfParallelism = 64
            });

            var link = new DataflowLinkOptions { PropagateCompletion = true };
            actionBlock.Post(modpackInfo.Files);
            actionBlock.Complete();

            await actionBlock.Completion;

            //资源安装 -2
            await Task.Run(() => {
                using (ZipArchive subPath = ZipFile.OpenRead(path)) {               
                    foreach (ZipArchiveEntry i in subPath.Entries.AsParallel()) {                   
                        if (i.FullName.StartsWith("overrides") && !string.IsNullOrEmpty(ZipExtension.GetString(subPath.GetEntry(i.FullName)))) {                       
                            string cutpath = i.FullName.Replace("overrides/", string.Empty);
                            FileInfo v = new FileInfo(Path.Combine(gamecorePath, cutpath));
                            if (!Directory.Exists(Path.Combine(gamecorePath, v.Directory.FullName))) {                           
                                Directory.CreateDirectory(Path.Combine(gamecorePath, v.Directory.FullName));
                            }
                            ZipExtension.ExtractTo(subPath.GetEntry(i.FullName), Path.Combine(gamecorePath, cutpath));
                        }
                    }
                }
            });


            data.TimerStop();
            $"整合包 {modpackInfo.Name} 安装成功！".ShowMessage("成功");
        }
    }
}
