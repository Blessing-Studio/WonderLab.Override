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
using WonderLab.Extensions;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class LaunchSettingPageViewModel : ViewModelBase {
    private readonly SettingData _data;
    private readonly LogService _logService;
    private readonly JavaFetcher _javaFetcher;
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;

    [ObservableProperty] private string _maxMemory;
    [ObservableProperty] private string _activeGameFolder;

    [ObservableProperty] private JavaEntry _activeJava;

    [ObservableProperty] private bool _isFullScreen;
    [ObservableProperty] private bool _isAutoSelectJava;
    [ObservableProperty] private bool _isGameIndependent;
    [ObservableProperty] private bool _isAutoAllocateMemory;

    [ObservableProperty] private ObservableCollection<JavaEntry> _javas;
    [ObservableProperty] private ObservableCollection<string> _gameFolders;

    public LaunchSettingPageViewModel(SettingService settingService, LogService logService, DialogService dialogService, JavaFetcher javaFetcher) {
        _logService = logService;
        _javaFetcher = javaFetcher;
        _dialogService = dialogService;
        _settingService = settingService;
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

            GameFolders.Add(folder.FullName);
            ActiveGameFolder = folder.FullName;
        } else {
            var java = await _dialogService.OpenFilePickerAsync(new List<FilePickerFileType> {
                new("JavaÎÄ¼þ") { Patterns = [EnvironmentUtil.IsWindow ? "javaw.exe" : "java"] }
            }, "Select Java");

            if (java is null) {
                return;
            }

            RunBackgroundWork(() => {
                var javaInfo = JavaUtil.GetJavaInfo(java.FullName);
                if (Javas.Any(x => x.JavaPath == javaInfo.JavaPath)) {
                    return;
                }

                Javas.Add(javaInfo);
                ActiveJava = javaInfo;
            });
        }
    }

    [RelayCommand]
    private void Remove(string key) {
        if (key is "Folder") {
            GameFolders.Remove(ActiveGameFolder);
            ActiveGameFolder = GameFolders.Any() ? GameFolders.First() : string.Empty;
            _logService.Info(nameof(LaunchSettingPageViewModel), $"Active game folder value is {ActiveGameFolder}");
        } else {
            Javas.Remove(ActiveJava);
            ActiveJava = Javas.Any() ? Javas.First() : null;
        }
    }

    [RelayCommand]
    private async Task AutoSearch() {
        var javas = await _javaFetcher.FetchAsync();
        if (Javas is null) {
            Javas = new();
        }

        Javas.Clear();

        var javasList = Javas?.Union(javas);
        foreach (var java in javasList) {
            Javas.Add(java);
        }

        ActiveJava = Javas.Last();
        _logService.Info(nameof(LaunchSettingPageViewModel), $"Current java count is {Javas.Count}");
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(Javas):
                _data.Javas = Javas.ToList();
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