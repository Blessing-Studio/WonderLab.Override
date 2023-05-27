using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;
using wonderlab.Views.Windows;

namespace wonderlab.Class.Models
{
    public class WebModpackModel {
        public WebModpackModel(CurseForgeModpack modpack) {
            NormalTitle = modpack.Name;
            IconUrl = modpack.IconUrl;
            LastUpdateTime = modpack.LastUpdateTime;
            Description = modpack.Description;
            ModpackSource = ModpackSource.Curseforge;
            GameVersions = modpack.ToString();
            Author = modpack.Authors.First().Name;
            DownloadCount = modpack.DownloadCount;
            ScreenshotUrls = modpack.Screenshots.Select(x => x.Url);
            Categories = modpack.Categories.Select(x => x.Name);

            GameVersions = modpack.SupportedVersions.Any() ? 
                (modpack.SupportedVersions.First() == modpack.SupportedVersions.Last() ?
                modpack.SupportedVersions.First() : $"{modpack.SupportedVersions.First()}-{modpack.SupportedVersions.Last()}") : "Unknown";

            string keyword = modpack.Links["websiteUrl"].TrimEnd('/').Split("/").Last();
            if (CacheResources.WebModpackInfoDatas.ContainsKey(keyword)) {           
                var result = CacheResources.WebModpackInfoDatas[keyword];
                if (!string.IsNullOrEmpty(result.Chinese)) {
                    ChineseTitle = result.Chinese;
                }
            } else ChineseTitle = modpack.Name;

            foreach (var i in modpack.Files.AsParallel()) {
                Files.Add(i.Key, i.Value.Select(x => new WebModpackFilesModel(x.FileName, x.DownloadUrl, $"{i.Key} 适用于 {x.ModLoaderType}")).ToObservableCollection());
            }
        }

        public WebModpackModel(ModrinthProjectInfoSearchResult info, List<ModrinthProjectInfoItem> files) {
            NormalTitle = info.Title;
            ChineseTitle = info.Title;
            IconUrl = info.IconUrl;
            LastUpdateTime = info.DateModified;
            Description = info.Description;
            DownloadCount = info.Downloads;
            ModpackSource = ModpackSource.Modrinth;
            Author = info.Author;
            Categories = info.Categories;

            GameVersions = files.Any() ?
                (files.First().GameVersion.First() == files.Last().GameVersion.Last() ? files.First().GameVersion.First() : $"{files.First().GameVersion.First()}-{files.Last().GameVersion.Last()}") : "Unknown";

            foreach (var x in files.AsParallel()) {
                if(!Files.ContainsKey(x.GameVersion.First())){
                    Files.Add(x.GameVersion.First(), x.Files.Select(x1 => new WebModpackFilesModel(x1.FileName, x1.Url, $"{x.GameVersion.First()} 适用于 {string.Join(", ", x.Loaders)}")).ToObservableCollection());
                }
            }
        }

        public string NormalTitle { get; set; }

        public string ChineseTitle { get; set; }

        public string IconUrl { get; set; }

        public string Description { get; set; }

        public int DownloadCount { get; set; }

        public string GameVersions { get; set; }

        public ModpackSource ModpackSource { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public string Author { get; set; }

        public IEnumerable<string> ScreenshotUrls { get; set; }

        public IEnumerable<string> Categories { get; set; }

        /// <summary>
        /// 备注：Key为支持的版本，Value为下载信息
        /// </summary>
        public Dictionary<string, ObservableCollection<WebModpackFilesModel>> Files { get; set; } = new();
    }

    public class WebModpackFilesModel {
        public WebModpackFilesModel(string name, string url, string loader)
        {
            Title = name;
            Url = url;
            Loader = loader;
        }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Loader { get; set; }

        public async void DownloadResourceAction() {
            SaveFileDialog dialog = new() {
                Title = "请选择文件保存路径",
                InitialFileName = Title
            };
            var result = await dialog.ShowAsync(App.CurrentWindow);
            if(string.IsNullOrEmpty(result)) {
                return;
            }

            $"开始下载资源 \"{Title}\"，您可以点击此条进入通知中心以查看下载进度！".ShowMessage(() => {
                App.CurrentWindow.NotificationCenter.Open();
            });

            NotificationViewData data = new() { 
                Title = $"资源 {Title} 的下载任务"
            };
            data.TimerStart();

            NotificationCenterPage.ViewModel.Notifications.Add(data);
            var doanloadResult = await HttpWrapper.HttpDownloadAsync(Url, result.Replace(Title, string.Empty), (e, _) => {
                var progress = e * 100;
                data.ProgressOfBar = progress;
                data.Progress = $"{Math.Round(progress, 2)}%";
            });

            data.TimerStop();
        }
    }
}
