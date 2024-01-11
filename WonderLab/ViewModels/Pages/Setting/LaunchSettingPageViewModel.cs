using System.Linq;
using WonderLab.Services;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using MinecraftLaunch.Utilities;
using System.Collections.Generic;
using WonderLab.Classes.Utilities;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MinecraftLaunch.Classes.Models.Game;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class LaunchSettingPageViewModel : ViewModelBase {
    private readonly DataService _dataService;

    public int MaxMemory {
        get => _dataService.ConfigData.MaxMemory;
        set => _dataService.ConfigData.MaxMemory = value;
    }

    public int Width {
        get => _dataService.ConfigData.Width;
        set => _dataService.ConfigData.Width = value;
    }

    public int Height {
        get => _dataService.ConfigData.Height;
        set => _dataService.ConfigData.Height = value;
    }
    
    public bool IsAutoMemory {
        get => _dataService.ConfigData.IsAutoMemory;
        set => _dataService.ConfigData.IsAutoMemory = value;
    }
    
    public bool IsFullscreen {
        get => _dataService.ConfigData.IsFullscreen;
        set => _dataService.ConfigData.IsFullscreen = value;
    }

    public bool IsAutoSelectJava {
        get => _dataService.ConfigData.IsAutoSelectJava;
        set => _dataService.ConfigData.IsAutoSelectJava = value;
    }

    public string GameFolder {
        get => _dataService.ConfigData.GameFolder;
        set => _dataService.ConfigData.GameFolder = value;
    }
    
    public JavaEntry JavaPath {
        get => _dataService.ConfigData.JavaPath;
        set => _dataService.ConfigData.JavaPath = value;
    }
    
    public bool IsEnableIndependencyCore {
        get => _dataService.ConfigData.IsEnableIndependencyCore;
        set => _dataService.ConfigData.IsEnableIndependencyCore = value;
    }
    
    public ObservableCollection<string> GameFolders {
        get => _dataService.ConfigData.GameFolders;
        set => _dataService.ConfigData.GameFolders = value;
    }
    
    public ObservableCollection<JavaEntry> JavaPaths {
        get => _dataService.ConfigData.JavaPaths;
        set => _dataService.ConfigData.JavaPaths = value;
    }

    public LaunchSettingPageViewModel(DataService dataService) {
        _dataService = dataService;
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