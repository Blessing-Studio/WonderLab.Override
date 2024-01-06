using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using MinecraftLaunch.Classes.Models.Download;
using MinecraftLaunch.Classes.Models.Event;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using WonderLab.Classes.Handlers;

namespace WonderLab.ViewModels.Dialogs {
    public partial class UpdateDialogContentViewModel : ViewModelBase {
        private readonly string _downloadUrl;
        private readonly UpdateHandler _updateHandler;
        private readonly DownloadHandler _downloadHandler;
        
        public UpdateDialogContentViewModel(UpdateHandler updateHandler, DownloadHandler downloadHandler) { 
            List<string> authors = new();
            _updateHandler = updateHandler;
            _downloadHandler = downloadHandler;
            
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

            string baseUrl = updateHandler.UpdateInfoJsonNode
                .GetString("windows_file_url");

            _downloadUrl = $"https://github.moeyy.xyz/{baseUrl}";
        }

        public string Time { get; }

        public string Author { get; }

        public string Messages { get; }

        [ObservableProperty]
        public double progress;

        [ObservableProperty]
        public bool isDownloading;

        [RelayCommand]
        private void Close() {
            DialogHost.Close("dialogHost");
        }

        [RelayCommand]
        private async void Update() {
            if (EnvironmentUtil.IsWindow) {
                IsDownloading = true;
                var downloadRequest = new DownloadRequest {
                    Url = _downloadUrl,
                    Name = "updateTemp.zip",
                    Path = Environment.CurrentDirectory
                };
                
                using var downloader = await _downloadHandler.DownloadAsync(downloadRequest);

                downloader.ProgressChanged += OnDownloadProgressChanged;
                using var zip = ZipFile.OpenRead(Path.Combine(downloadRequest.Path, 
                    downloadRequest.Name));
                
                var launcherEntry = zip.Entries
                    .FirstOrDefault(x => x.Name.ToLower() == "wonderlab.exe");
                if (launcherEntry is not null) {
                    launcherEntry.ExtractToFile(Path.Combine(downloadRequest.Path,
                        "launcher.temp"));

                    _updateHandler.Update();
                }
            }
        }

        private void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e) {
            Progress = e.ToPercentage() * 100;
        }
    }
}
