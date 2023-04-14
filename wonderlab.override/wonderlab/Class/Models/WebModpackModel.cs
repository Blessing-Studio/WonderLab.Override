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
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;

namespace wonderlab.Class.Models
{
    public class WebModpackModel {
        public WebModpackModel(CurseForgeModpack modpack) {
            NormalTitle = modpack.Name;
            IconUrl = modpack.IconUrl;
            CreatedAt = modpack.LastUpdateTime;
            Description = modpack.Description;
            ModpackSource = ModpackSource.Curseforge;
            GameVersions = modpack.ToString();
            DownloadCount = modpack.DownloadCount.ToString();
            GameVersions = modpack.SupportedVersions.Any() ? 
                (modpack.SupportedVersions.First() == modpack.SupportedVersions.Last() ?
                modpack.SupportedVersions.First() : $"{modpack.SupportedVersions.First()}-{modpack.SupportedVersions.Last()}") : "Unknown";

            string keyword = modpack.Links["websiteUrl"].TrimEnd('/').Split("/").Last();
            if (DataUtil.WebModpackInfoDatas.ContainsKey(keyword)) {           
                var result = DataUtil.WebModpackInfoDatas[keyword];
                if (!string.IsNullOrEmpty(result.Chinese))
                    ChineseTitle = result.Chinese;
            }

            //ThreadPool.QueueUserWorkItem(x => {

            //    string keyword = modpack.Name ??= string.Empty;
            //    if (DataUtil.WebModpackInfoDatas.ContainsKey(keyword)) {               
            //        var result = DataUtil.WebModpackInfoDatas[keyword];
            //        if (!string.IsNullOrEmpty(result.Chinese))
            //            ChineseTitle = result.Chinese;
            //    }
            //});

            foreach (var i in modpack.Files.AsParallel()) {
                Files.Add(i.Key, i.Value.Select(x => new WebModpackFilesModel(x.FileName, x.DownloadUrl)).ToObservableCollection());
            }
        }

        public WebModpackModel(ModrinthProjectInfoSearchResult info, List<ModrinthProjectInfoItem> files) {
            NormalTitle = info.Title;
            IconUrl = info.IconUrl;
            CreatedAt = info.DateCreated;
            Description = info.Description;
            DownloadCount = info.Downloads.ToString();
            ModpackSource = ModpackSource.Modrinth;
            GameVersions = files.Any() ?
                (files.First().GameVersion.First() == files.Last().GameVersion.Last() ? files.First().GameVersion.First() : $"{files.First().GameVersion.First()}-{files.Last().GameVersion.Last()}") : "Unknown";

            foreach (var x in files.AsParallel()) {
                if(!Files.ContainsKey(x.GameVersion.First())){
                    Files.Add(x.GameVersion.First(), x.Files.Select(x => new WebModpackFilesModel(x.FileName, x.Url)).ToObservableCollection());
                }
            }
        }

        public string NormalTitle { get; set; }

        public string ChineseTitle { get; set; }

        public string IconUrl { get; set; }

        public string Description { get; set; }

        public string DownloadCount { get; set; }

        public string GameVersions { get; set; }

        public ModpackSource ModpackSource { get; set; }

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 备注：Key为支持的版本，Value为下载信息
        /// </summary>
        public Dictionary<string, ObservableCollection<WebModpackFilesModel>> Files { get; set; } = new();
    }

    public class WebModpackFilesModel {
        public WebModpackFilesModel(string name, string url) {
            Title = name;
            Url = url;
        }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}
