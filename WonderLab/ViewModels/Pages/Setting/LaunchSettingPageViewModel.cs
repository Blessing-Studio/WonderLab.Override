using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using WonderLab.Classes.Managers;
using WonderLab.Classes.Utilities;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Attributes;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Utilities;

namespace WonderLab.ViewModels.Pages.Setting {
    public partial class LaunchSettingPageViewModel : ViewModelBase {
        private DataManager _configDataManager;

        [ObservableProperty]
        [BindToConfig("GameFolder")]
        public string gameFolder;

        [ObservableProperty]
        [BindToConfig("GameFolders")]
        public ObservableCollection<string> gameFolders;

        [ObservableProperty]
        [BindToConfig("JavaPath")]
        public JavaEntry javaPath;

        [ObservableProperty]
        [BindToConfig("JavaPaths")]
        public ObservableCollection<JavaEntry> javaPaths;

        [ObservableProperty]
        [BindToConfig("IsAutoSelectJava")]
        public bool isAutoSelectJava;

        [ObservableProperty]
        [BindToConfig("MaxMemory")]
        public int maxMemory;

        [ObservableProperty]
        [BindToConfig("Width")]
        public int width;

        [ObservableProperty]
        [BindToConfig("Height")]
        public int height;

        [ObservableProperty]
        [BindToConfig("IsAutoMemory")]
        public bool isAutoMemory;

        [ObservableProperty]
        [BindToConfig("IsFullscreen")]
        public bool isFullscreen;

        [ObservableProperty]
        [BindToConfig("IsEnableIndependencyCore")]
        public bool isEnableIndependencyCore;

        public LaunchSettingPageViewModel(DataManager manager) : base(manager) {
            _configDataManager = manager;
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
                => await _configDataManager.JavaFetcher.FetchAsync());

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
}
