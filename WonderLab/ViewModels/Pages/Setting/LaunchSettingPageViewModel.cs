using System.Linq;
using WonderLab.Services;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using MinecraftLaunch.Utilities;
using System.Collections.Generic;
using WonderLab.Classes.Utilities;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Attributes;
using System.Collections.ObjectModel;
using MinecraftLaunch.Classes.Models.Game;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Pages.Setting;

public partial class LaunchSettingPageViewModel : ViewModelBase {
    private readonly DataService _dataService;

    [ObservableProperty]
    [BindToConfig("GameFolder")]
    private string gameFolder;

    [ObservableProperty]
    [BindToConfig("GameFolders")]
    private ObservableCollection<string> gameFolders;

    [ObservableProperty]
    [BindToConfig("JavaPath")]
    private JavaEntry javaPath;

    [ObservableProperty]
    [BindToConfig("JavaPaths")]
    private ObservableCollection<JavaEntry> javaPaths;

    [ObservableProperty]
    [BindToConfig("IsAutoSelectJava")]
    private bool isAutoSelectJava;

    [ObservableProperty]
    [BindToConfig("MaxMemory")]
    private int maxMemory;

    [ObservableProperty]
    [BindToConfig("Width")]
    private int width;

    [ObservableProperty]
    [BindToConfig("Height")]
    private int height;

    [ObservableProperty]
    [BindToConfig("IsAutoMemory")]
    private bool isAutoMemory;

    [ObservableProperty]
    [BindToConfig("IsFullscreen")]
    private bool isFullscreen;

    [ObservableProperty]
    [BindToConfig("IsEnableIndependencyCore")]
    private bool isEnableIndependencyCore;

    public LaunchSettingPageViewModel(DataService dataService) {
        _dataService = dataService;
        JavaPath = JavaPaths?.FirstOrDefault(x => x.JavaPath == JavaPath?.JavaPath)!;
    }

    [RelayCommand]
    private async Task AddGameFolder() {
        try {
            var result = await DialogUtil.OpenFolderPickerAsync("选择 .minecraft 文件夹");

            if (result != null) {
                GameFolders.Add(result.FullName);
                GameFolder = GameFolders.Last();
            }
        }
        catch (System.Exception) {
            // ignored
        }
    }

    [RelayCommand]
    private void RemoveGameFolder() {
        GameFolders.Remove(GameFolder);
        GameFolder = GameFolders.Any() ? GameFolders.Last() : null!;
    }

    [RelayCommand]
    private async Task AddJavaPath() {
        var result = await DialogUtil.OpenFilePickerAsync(new List<FilePickerFileType>() {
            new("Java文件") { Patterns = new List<string>() { EnvironmentUtil.IsWindow ? "javaw.exe" : "java" } }
        }, "请选择您的 Java 文件");

        if (result != null) {
            JavaPaths.Add(JavaUtil.GetJavaInfo(result.FullName));
            JavaPath = JavaPaths.Last();
        }
    }

    [RelayCommand]
    private async Task AddJavaPaths() {
        var result = await Task.Run(async () 
            => await _dataService.javaFetcher.FetchAsync());

        foreach (var item in result) {
            JavaPaths.Add(item);
        }
        JavaPath = JavaPaths.Last();
    }

    [RelayCommand]
    private void RemoveJavaPath() {
        JavaPaths.Remove(JavaPath);
        JavaPath = JavaPaths.Any() ? JavaPaths.Last() : null!;
    }
}