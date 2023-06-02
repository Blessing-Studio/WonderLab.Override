using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using DynamicData;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.Views.Windows;

namespace wonderlab.ViewModels.Pages {
    public class LaunchConfigPageViewModel : ViewModelBase {
        public LaunchConfigPageViewModel() {
            PropertyChanged += OnPropertyChanged;

            try {
                if (GlobalResources.LaunchInfoData.JavaRuntimes.Any()) {
                    ThreadPool.QueueUserWorkItem(x => {
                        Javas = GlobalResources.LaunchInfoData.JavaRuntimes.ToObservableCollection();
                        CurrentJava = Javas.Where(x => {
                            if (x != null) {
                                if (x.JavaPath.ToJavaw() == GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw()) {
                                    return true;
                                }
                            }
                            return false;
                        })?.First()!;
                    });
                }

                if (GlobalResources.LaunchInfoData.GameDirectorys.Any()) {
                    CurrentGameDirectory = GlobalResources.LaunchInfoData.GameDirectoryPath;
                    GameDirectorys = GlobalResources.LaunchInfoData.GameDirectorys.ToObservableCollection();
                }

                WindowHeight = GlobalResources.LaunchInfoData.WindowHeight;
                WindowWidth = GlobalResources.LaunchInfoData.WindowWidth;
                IsAutoSelectJava = GlobalResources.LaunchInfoData.IsAutoSelectJava;
                IsAutoGetMemory = GlobalResources.LaunchInfoData.IsAutoGetMemory;
                MaxMemory = GlobalResources.LaunchInfoData.MaxMemory;
            }
            catch (Exception ex) {
                $"{ex.Message}".ShowMessage();
            }
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(Javas)) {
                GlobalResources.LaunchInfoData.JavaRuntimes = Javas.ToList();
            }

            if (e.PropertyName is nameof(GameDirectorys)) {
                GlobalResources.LaunchInfoData.GameDirectorys = GameDirectorys.ToList();
            }

            if (e.PropertyName is nameof(CurrentJava)) {
                GlobalResources.LaunchInfoData.JavaRuntimePath = CurrentJava;
            }

            if (e.PropertyName is nameof(CurrentGameDirectory)) {
                GlobalResources.LaunchInfoData.GameDirectoryPath = CurrentGameDirectory;
            }

            if (e.PropertyName is nameof(MaxMemory)) {
                GlobalResources.LaunchInfoData.MaxMemory = MaxMemory;
            }

            if (e.PropertyName is nameof(IsAutoSelectJava)) {
                GlobalResources.LaunchInfoData.IsAutoSelectJava = IsAutoSelectJava;
            }

            if (e.PropertyName is nameof(WindowHeight)) {
                GlobalResources.LaunchInfoData.WindowHeight = WindowHeight;
            }

            if (e.PropertyName is nameof(WindowWidth)) {
                GlobalResources.LaunchInfoData.WindowWidth = WindowWidth;
            }

            if (e.PropertyName is nameof(IsAutoGetMemory)) {
                GlobalResources.LaunchInfoData.IsAutoGetMemory = IsAutoGetMemory;
            }
        }

        [Reactive]
        public bool IsLoadJavaFinish { get; set; } = true;

        [Reactive]
        public bool IsAutoSelectJava { get; set; } = false;

        [Reactive]
        public bool IsAutoGetMemory { get; set; } = true;

        [Reactive]
        public bool IsLoadJavaNow { get; set; } = false;

        [Reactive]
        public int MaxMemory { get; set; }

        [Reactive]
        public int WindowWidth { get; set; } = 854;

        [Reactive]
        public int WindowHeight { get; set; } = 480;

        [Reactive]
        public int CurrentAction { get; set; } = 1;

        [Reactive]
        public JavaInfo CurrentJava { get; set; }

        [Reactive]
        public string CurrentGameDirectory { get; set; }

        [Reactive]
        public ObservableCollection<JavaInfo> Javas { get; set; } = new();

        [Reactive]
        public ObservableCollection<string> GameDirectorys { get; set; } = new();

        public ObservableCollection<string> Actions => new()
        {
            "最小化",
            "无任何行为",
            "关闭 WonderLab",
        };

        public async void LoadJavaAction() {
            IsLoadJavaFinish = false;
            IsLoadJavaNow = true;

            await Task.Run(async () => {
                if (SystemUtils.IsWindows) {
                    foreach (var drive in DriveInfo.GetDrives().AsParallel()) {
                        FileSearchAsync(drive.RootDirectory, "javaw.exe");
                    }
                }
                else {
                    var result = await JavaUtils.GetJavas().ToListAsync();
                    if (!result.IsNull()) {
                        foreach (var java in result) {
                            Javas.Add(java);
                            await Task.Delay(10);
                        }
                    }
                }

                IsLoadJavaFinish = true;
                IsLoadJavaNow = false;
            });
        }

        public void FileSearchAsync(DirectoryInfo directory, string pattern) {
            try {
                foreach (FileInfo fi in directory.GetFiles(pattern).Where(x => !x.IsReadOnly
                && directory.Attributes != FileAttributes.ReadOnly && directory.Attributes != FileAttributes.System
                && directory.Attributes != FileAttributes.Hidden)) {
                    AddRelativeDocument(fi.FullName);
                }

                foreach (DirectoryInfo di in directory.GetDirectories().Where(x => directory.Attributes != FileAttributes.ReadOnly)) {
                    if (!di.FullName.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Windows))) {
                        FileSearchAsync(di, pattern);
                    }
                }
            }
            catch (Exception) {

            }
        }

        public void AddRelativeDocument(string path) {
            FileInfo fileInfo = new FileInfo(path);
            DirectoryInfo? directory = fileInfo.Directory;
            if (directory != null) {
                var javaInfo = JavaToolkit.GetJavaInfo(Path.Combine(path1: directory.FullName, "java.exe"));
                Javas.Add(javaInfo);
                GlobalResources.LaunchInfoData.JavaRuntimes.Add(JavaToolkit.GetJavaInfo(path));
                CurrentJava = javaInfo;
                Trace.WriteLine($"[信息] 这是第 {Javas.Count} 找到的 Java 运行时，完整路径为 {path}");
            }
        }

        public async void AddJavaAction() {
            var file = await DialogUtils.OpenFilePickerAsync(new List<FilePickerFileType>() {
                new("Java文件") { Patterns = new List<string>() { SystemUtils.IsWindows ? "javaw.exe" : "java" } }
            }, "请选择您的 Java 文件");

            if (!file.IsNull()) {
                //由于需启动新进程，可能耗时会卡主线程，因此使用异步
                var java = await Task.Run(() => JavaToolkit.GetJavaInfo(file.FullName));

                if (!java.IsNull()) {
                    Javas.Add(java);
                    GlobalResources.LaunchInfoData.JavaRuntimes.Add(java);
                    CurrentJava = java;
                }
            }
        }

        public async void DirectoryDialogOpenAction() {
            var folder = await DialogUtils.OpenFolderPickerAsync("请选择一个游戏目录");
            if (!folder.IsNull()) {
                GameDirectorys.Add(folder.FullName);
                GlobalResources.LaunchInfoData.GameDirectorys.Add(folder.FullName);
                CurrentGameDirectory = folder.FullName;
            }
        }

        public void RemoveDirectoryAction() {
            GameDirectorys.Remove(CurrentGameDirectory);
            CurrentGameDirectory = GameDirectorys.Any() ? GameDirectorys.First() : string.Empty;
        }

        public void RemoveJavaRuntimeAction() {
            Javas.Remove(CurrentJava);
            CurrentJava = Javas.Any() ? Javas.First() : null!;
        }

        public void CloseAutoSelectJavaAction() {
            IsAutoSelectJava = false;
        }

        public void OpenAutoSelectJavaAction() {
            IsAutoSelectJava = true;
        }

        public void CloseAutoGetMemoryAction() {
            IsAutoGetMemory = false;
        }

        public void OpenAutoGetMemoryAction() {
            IsAutoGetMemory = true;
        }
    }
}
