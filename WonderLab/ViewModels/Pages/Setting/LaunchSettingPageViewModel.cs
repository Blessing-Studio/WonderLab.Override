using ReactiveUI;
using System.Linq;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using WonderLab.Classes.Managers;
using WonderLab.Classes.Utilities;
using WonderLab.Classes.Attributes;
using System.Collections.ObjectModel;
using MinecraftLaunch.Modules.Utilities;
using MinecraftLaunch.Modules.Models.Launch;
using System.Threading.Tasks;
using DynamicData;

namespace WonderLab.ViewModels.Pages.Setting {
    public class LaunchSettingPageViewModel : ViewModelBase {
        private DataManager _configDataManager;

        [Reactive]
        [BindToConfig("GameFolder")]
        public string GameFolder { get; set; }

        [Reactive]
        [BindToConfig("GameFolders")]
        public ObservableCollection<string> GameFolders { get; set; }
        
        [Reactive]
        [BindToConfig("JavaPath")]
        public JavaInfo JavaPath { get; set; }

        [Reactive]
        [BindToConfig("JavaPaths")]
        public ObservableCollection<JavaInfo> JavaPaths { get; set; }
        
        [Reactive]
        [BindToConfig("IsAutoSelectJava")]
        public bool IsAutoSelectJava { get; set; }

        [Reactive]
        [BindToConfig("MaxMemory")]
        public int MaxMemory { get; set; }

        [Reactive]
        [BindToConfig("Width")]
        public int Width { get; set; }

        [Reactive]
        [BindToConfig("Height")]
        public int Height { get; set; }

        [Reactive]
        [BindToConfig("IsAutoMemory")]
        public bool IsAutoMemory { get; set; }

        [Reactive]
        [BindToConfig("IsFullscreen")]
        public bool IsFullscreen { get; set; }

        [Reactive]
        [BindToConfig("IsEnableIndependencyCore")]
        public bool IsEnableIndependencyCore { get; set; }

        public ICommand AddGameFolderCommand =>
            ReactiveCommand.Create(AddGameFolder);

        public ICommand RemoveGameFolderCommand =>
            ReactiveCommand.Create(RemoveGameFolder);

        public ICommand AddJavaPathCommand =>
            ReactiveCommand.Create(AddJavaPath);

        public ICommand AddJavaPathsCommand =>
            ReactiveCommand.Create(AddJavaPaths);

        public ICommand RemoveJavaPathCommand =>
            ReactiveCommand.Create(RemoveJavaPath);

        public LaunchSettingPageViewModel(DataManager manager) : base(manager) {
            _configDataManager = manager;
            JavaPath = JavaPaths?.FirstOrDefault(x => x.JavaPath == JavaPath?.JavaPath)!;
        }

        private async void AddGameFolder() {
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

        private void RemoveGameFolder() {
            GameFolders.Remove(GameFolder);
            GameFolder = GameFolders.Any() ? GameFolders.Last() : null!;
        }

        private async void AddJavaPath() {
            var result = await DialogUtil.OpenFilePickerAsync(new List<FilePickerFileType>() {
                new("Java文件") { Patterns = new List<string>() { EnvironmentUtil.IsWindow ? "javaw.exe" : "java" } }
            }, "请选择您的 Java 文件");

            if (result != null) {
                JavaPaths.Add(JavaUtil.GetJavaInfo(result.FullName));
                JavaPath = JavaPaths.Last();
            }
        }

        private async void AddJavaPaths() {
            await Task.Run(JavaUtil.GetJavas)
                .ContinueWith(async javasTask => {
                    var javas = await javasTask;

                    JavaPaths.AddRange(javas);
                    JavaPath = JavaPaths.Last();
                });
        }

        private void RemoveJavaPath() {
            JavaPaths.Remove(JavaPath);
            JavaPath = JavaPaths.Any() ? JavaPaths.Last() : null!;
        }
    }
}
