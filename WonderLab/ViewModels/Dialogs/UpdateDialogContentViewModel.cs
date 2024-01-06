using System;
using System.IO;
using System.Linq;
using WonderLab.Services;
using DialogHostAvalonia;
using System.IO.Compression;
using System.Threading.Tasks;
using MinecraftLaunch.Utilities;
using MinecraftLaunch.Extensions;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Classes.Models.Event;
using MinecraftLaunch.Classes.Models.Download;

namespace WonderLab.ViewModels.Dialogs;

public partial class UpdateDialogContentViewModel : ViewModelBase {
    private readonly string _downloadUrl;
    private readonly UpdateService _updateService;
    private readonly DownloadService _downloadService;
        
    public UpdateDialogContentViewModel(UpdateService updateService, DownloadService downloadService) { 
        List<string> authors = new();
        _updateService = updateService;
        _downloadService = downloadService;
            
        var messages = _updateService.UpdateInfoJsonNode
            .GetEnumerable("messages")
            .Select(x => {
                var msg = x!.GetValue<string>();
                var authorMatch = Regex.Match(msg, @"\((.*?)\)");
                if (authorMatch.Success) {
                    authors.Add(authorMatch.Groups[1].Value);
                }

                return Regex.Replace(msg, @"-\s(\w+):\s(.+)\s\(.+",
                    "- $2 ($1)");
            }).ToList();

        Time = _updateService.UpdateInfoJsonNode["time"]!
            .GetValue<DateTime>()
            .ToString("F");

        Messages = string.Join("\n", messages);
        Author = string.Join(", ", authors.Distinct());

        string baseUrl = _updateService.UpdateInfoJsonNode
            .GetString("windows_file_url");

        _downloadUrl = $"https://github.moeyy.xyz/{baseUrl}";
    }

    public string Time { get; }

    public string Author { get; }

    public string Messages { get; }

    [ObservableProperty] 
    private double progress;

    [ObservableProperty] 
    private bool isDownloading;

    [RelayCommand]
    private void Close() {
        DialogHost.Close("dialogHost");
    }

    [RelayCommand]
    private async Task Update() {
        if (EnvironmentUtil.IsWindow) {
            IsDownloading = true;
            var downloadRequest = new DownloadRequest {
                Url = _downloadUrl,
                Name = "updateTemp.zip",
                Path = Environment.CurrentDirectory
            };
                
            using var downloader = await _downloadService.DownloadAsync(downloadRequest);

            downloader.ProgressChanged += OnDownloadProgressChanged;
            using var zip = ZipFile.OpenRead(Path.Combine(downloadRequest.Path, 
                downloadRequest.Name));
                
            var launcherEntry = zip.Entries
                .FirstOrDefault(x => x.Name.ToLower() == "wonderlab.exe");
            if (launcherEntry is not null) {
                launcherEntry.ExtractToFile(Path.Combine(downloadRequest.Path,
                    "launcher.temp"));

                _updateService.Update();
            }
        }
    }

    private void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e) {
        Progress = e.ToPercentage() * 100;
    }
}