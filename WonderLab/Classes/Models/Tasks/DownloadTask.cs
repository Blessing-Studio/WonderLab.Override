using Microsoft.Extensions.Logging;
using MinecraftLaunch.Modules.Downloaders;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Models.Tasks {
    public class DownloadTask : TaskBase {
        private DownloadRequest _downloadRequest = default!;

        public DownloadTask(string uri, DirectoryInfo Directory, string fileName = null) {
            _downloadRequest = new() {
                Url = uri,
                FileName = fileName,
                Directory = Directory,
            };

            JobName = $"文件 {fileName} 的下载任务";
        }

        public override async ValueTask BuildWorkItemAsync(CancellationToken token) {
            base.CanBeCancelled = false;

            await Task.Delay(2000);
            base.IsIndeterminate = false;

            GameCoreUtil.GetGameCore("C:\\Users\\w\\Desktop\\temp\\.minecraft", "1.12.2");

            var installer = await Task.Run(() => new GameCoreInstaller("C:\\Users\\w\\Desktop\\temp\\.minecraft", "1.20.2"));
            installer.ProgressChanged += (_, args) => {
                ReportProgress(args.Progress * 100, $"{args.Progress * 100:0.00}%");
            };

            var result = await installer.InstallAsync();

            if (result.Success) {
                Progress = 100;
                ProgressDetail = "100%";
            }
            //using var downloader = FileDownloader.Build(_downloadRequest);
            //downloader.DownloadFailed += (_, args) => {
            //    Debug.WriteLine(args.Message);
            //};

            //downloader.DownloadCompleted += (_, args) => {
            //    Progress = 100;
            //    ProgressDetail = "100%";
            //};

            //downloader.DownloadProgressChanged += (_, args) => {
            //    ReportProgress(args.Progress * 100, $"{args.Progress * 100:0.00}%");
            //};

            //base.IsIndeterminate = false;
            //downloader.BeginDownload();
            //await downloader.CompleteAsync();
        }
    }
}
