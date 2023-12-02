using Avalonia.ReactiveUI;
using DynamicData;
using MinecraftLaunch.Modules.Downloaders;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using WonderLab.Classes.Handlers;

namespace WonderLab.ViewModels.Dialogs {
    public class UpdateDialogContentViewModel : ViewModelBase {
        private string _downloadUrl;

        private UpdateHandler _updateHandler;

        public UpdateDialogContentViewModel(UpdateHandler updateHandler) { 
            List<string> authors = new();
            _updateHandler = updateHandler;

            var messages = updateHandler.UpdateInfoJsonNode["messages"]!
                .AsArray()
                .Select(x => {
                    var msg = x!.GetValue<string>();
                    var authorMatch = Regex.Match(msg, @"\((.*?)\)");
                    if (authorMatch.Success) {
                        authors.Add(authorMatch.Groups[1].Value);
                    }

                    return Regex.Replace(msg, @"-\s(\w+):\s(.+)\s\(.+",
                        "- $2 ($1)");
                }).ToList();

            Time = updateHandler.UpdateInfoJsonNode["time"]!
                .GetValue<DateTime>()
                .ToString("F");

            Messages = string.Join("\n", messages);
            Author = string.Join(", ", authors.Distinct());

            string baseUrl = updateHandler.UpdateInfoJsonNode["windows_file_url"]!
                .GetValue<string>();

            _downloadUrl = $"https://github.moeyy.xyz/{baseUrl}";
        }

        public string Time { get; }

        public string Author { get; }

        public string Messages { get; }

        [Reactive]
        public double Progress { get; set; }

        [Reactive]
        public bool IsDownloading { get; set; }

        public ICommand UpdateCommand =>
            ReactiveCommand.Create(Update);

        private async void Update() {
            IsDownloading = true;
            using var downloader = FileDownloader.Build(new DownloadRequest {
                Url = _downloadUrl,
                FileName = "updateTemp.zip",
                Directory = new(Environment.CurrentDirectory)
            });

            downloader.DownloadProgressChanged += OnDownloadProgressChanged;
            downloader.BeginDownload();
            var result = await downloader.CompleteAsync();
            if (result.Success) {
                using var zip = ZipFile.OpenRead(result.Result.FullName);
                var launcherEntry = zip.Entries
                    .FirstOrDefault(x => x.Name.ToLower() == "wonderlab.exe");
                if (launcherEntry is not null) {
                    launcherEntry.ExtractToFile(Path.Combine(result.Result.Directory.FullName,
                        "launcher.temp"));

                    _updateHandler.Update();
                }
            }
        }

        private void OnDownloadProgressChanged(object? sender, FileDownloaderProgressChangedEventArgs e) {
            Progress = e.Progress * 100;
        }
    }
}
