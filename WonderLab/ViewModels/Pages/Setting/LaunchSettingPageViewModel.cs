using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Components.Fetcher;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Datas;
using System.Threading.Tasks;
using WonderLab.Services.UI;
using WonderLab.Services;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Platform.Storage;
using MinecraftLaunch.Utilities;
using Avalonia.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MinecraftLaunch.Classes.Models.Game;
using System;
using System.IO;
using WonderLab.Extensions;
using Microsoft.Extensions.Logging;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class LaunchSettingPageViewModel : ViewModelBase {
    private readonly SettingData _data;
    private readonly JavaFetcher _javaFetcher;
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly ILogger<LaunchSettingPageViewModel> _logger;   

    [ObservableProperty] private string _maxMemory;
    [ObservableProperty] private string _activeGameFolder;

    [ObservableProperty] private JavaEntry _activeJava;

    [ObservableProperty] private bool _isFullScreen;
    [ObservableProperty] private bool _isAutoSelectJava;
    [ObservableProperty] private bool _isGameIndependent;
    [ObservableProperty] private bool _isAutoAllocateMemory;

    [ObservableProperty] private ObservableCollection<JavaEntry> _javas;
    [ObservableProperty] private ObservableCollection<string> _gameFolders;

    public LaunchSettingPageViewModel(
        JavaFetcher javaFetcher,
        DialogService dialogService, 
        SettingService settingService, 
        ILogger<LaunchSettingPageViewModel> logger) {
        _dialogService = dialogService;
        _settingService = settingService;

        _logger = logger;
        _javaFetcher = javaFetcher;
        _data = _settingService.Data;

        ActiveJava = _data.ActiveJava;
        Javas = _data.Javas.ToObservableList();
        MaxMemory = _data.MaxMemory.ToString();
        ActiveGameFolder = _data.ActiveGameFolder;
        GameFolders = _data.GameFolders.ToObservableList();

        IsFullScreen = _data.IsFullScreen;
        IsAutoSelectJava = _data.IsAutoSelectJava;
        IsGameIndependent = _data.IsGameIndependent;
        IsAutoAllocateMemory = _data.IsAutoAllocateMemory;
    }

    [RelayCommand]
    private async Task Search(string key) {
        if (key is "Folder") {
            var folder = await _dialogService.OpenFolderPickerAsync("Select Folder");
            if (folder is null) {
                return;
            }

            if (GameFolders.Any(x => x == folder.FullName)) {
                return;
            }

            GameFolders = GameFolders.Union(Enumerable.Repeat(folder.FullName, 1)).ToObservableList();
            ActiveGameFolder = GameFolders.Last();
        } else {
            var java = await _dialogService.OpenFilePickerAsync(new List<FilePickerFileType> {
                new("Java文件") { Patterns = [EnvironmentUtil.IsWindow ? "javaw.exe" : "java"] }
            }, "Select Java");

            if (java is null) {
                return;
            }

            RunBackgroundWork(() => {
                string javaPath = java.Name is "jre.bundle" 
                    ? Path.Combine(java.FullName, "Contents", "Home", "bin", "java")
                    : java.FullName;
                
                var javaInfo = JavaUtil.GetJavaInfo(javaPath);
                if (Javas.Count > 0 && Javas.Any(x => x?.JavaPath == javaInfo.JavaPath)) {
                    return;
                }

                Javas = Javas.Union(Enumerable.Repeat(javaInfo, 1)).ToObservableList();
                ActiveJava = Javas.Last();
            });
        }
    }

    [RelayCommand]
    private void Remove(string key) {
        if (key is "Folder") {
            GameFolders.Remove(ActiveGameFolder);
            GameFolders = GameFolders.ToObservableList();
            ActiveGameFolder = GameFolders.Any() ? GameFolders.First() : string.Empty;
            _logger.LogInformation("活动游戏目录为 {ActiveGameFolder}", ActiveGameFolder);
        } else {
            Javas.Remove(ActiveJava);
            ActiveJava = Javas.Any() ? Javas.First() : null;
            Javas = Javas.ToObservableList();
        }
    }

    [RelayCommand]
    private async Task AutoSearch() {
        var javas = await _javaFetcher.FetchAsync();
        Javas ??= [];
        Javas.Clear();

        var javasList = Javas?.Union(javas);
        Javas = javasList.ToObservableList();

        ActiveJava = Javas.LastOrDefault();
        _logger.LogInformation("共存在 {JavaCount} 个 Java", Javas.Count);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(Javas):
                _data.Javas = [.. Javas];
                break;
            case nameof(ActiveJava):
                _data.ActiveJava = ActiveJava;
                break;
            case nameof(GameFolders):
                _data.GameFolders = [.. GameFolders];
                break;
            case nameof(ActiveGameFolder):
                _data.ActiveGameFolder = ActiveGameFolder;
                break;
            case nameof(IsFullScreen):
                _data.IsFullScreen = IsFullScreen;
                break;
            case nameof(IsAutoSelectJava):
                _data.IsAutoSelectJava = IsAutoSelectJava;
                break;
            case nameof(IsGameIndependent):
                _data.IsGameIndependent = IsGameIndependent;
                break;
            case nameof(IsAutoAllocateMemory):
                _data.IsAutoAllocateMemory = IsAutoAllocateMemory;
                break;
            case nameof(MaxMemory):
                if (int.TryParse(MaxMemory, out var maxMemory)) {
                    _data.MaxMemory = maxMemory;
                }
                break;
        }
    }
}