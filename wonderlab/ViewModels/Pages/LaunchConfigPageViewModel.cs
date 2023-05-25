using Avalonia.Controls;
using Avalonia.Media;
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
using wonderlab.Class.Utils;

namespace wonderlab.ViewModels.Pages
{
    public class LaunchConfigPageViewModel : ReactiveObject {
        public LaunchConfigPageViewModel() {
            PropertyChanged += OnPropertyChanged;

            if (App.LaunchInfoData.JavaRuntimes.Any()) {
                ThreadPool.QueueUserWorkItem(x => {               
                    Javas = App.LaunchInfoData.JavaRuntimes.ToObservableCollection();
                    CurrentJava = Javas.Where(x => {                 
                        if (x != null) {                       
                            if (x.JavaPath.ToJavaw() == App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw()) {                           
                                return true;
                            }
                        }
                        return false;
                    })?.First()!;
                });
            }

            if (App.LaunchInfoData.GameDirectorys.Any()) {
                CurrentGameDirectory = App.LaunchInfoData.GameDirectoryPath;
                GameDirectorys = App.LaunchInfoData.GameDirectorys.ToObservableCollection();
            }

            IsAutoSelectJava = App.LaunchInfoData.IsAutoSelectJava;
            MaxMemory = App.LaunchInfoData.MaxMemory;
            MiniMemory = App.LaunchInfoData.MiniMemory;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Javas)) {
                App.LaunchInfoData.JavaRuntimes = Javas.ToList();
            }

            if (e.PropertyName == nameof(GameDirectorys)) { 
                App.LaunchInfoData.GameDirectorys = GameDirectorys.ToList();
            }

            if(e.PropertyName == nameof(CurrentJava)) { 
                App.LaunchInfoData.JavaRuntimePath = CurrentJava;
            }

            if (e.PropertyName == nameof(CurrentGameDirectory)) {
                App.LaunchInfoData.GameDirectoryPath = CurrentGameDirectory;
            }

            if (e.PropertyName == nameof(MaxMemory)) {
                App.LaunchInfoData.MaxMemory = MaxMemory;
            }

            if (e.PropertyName == nameof(MiniMemory)) {           
                App.LaunchInfoData.MiniMemory = MiniMemory;
            }

            if (e.PropertyName == nameof(IsAutoSelectJava)) {           
                App.LaunchInfoData.IsAutoSelectJava = IsAutoSelectJava;
            }
        }
        
        [Reactive]
        public bool IsLoadJavaFinish { get; set; } = true;

        [Reactive]
        public bool IsAutoSelectJava { get; set; } = false;

        [Reactive]
        public bool IsLoadJavaNow { get; set; } = false;

        [Reactive]
        public int MaxMemory { get; set; }

        [Reactive]
        public int MiniMemory { get; set; }

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

            await Task.Run(() => {
                DriveInfo.GetDrives().ToList().ForEach(x => {
                    FileSearchAsync(x.RootDirectory, "javaw.exe");
                });

                IsLoadJavaFinish = true;
                IsLoadJavaNow = false;
            });
        }

        public void FileSearchAsync(DirectoryInfo directory, string pattern)
        {
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
                App.LaunchInfoData.JavaRuntimes.Add(JavaToolkit.GetJavaInfo(path));
                CurrentJava = javaInfo;
                Trace.WriteLine($"[信息] 这是第 {Javas.Count} 找到的 Java 运行时，完整路径为 {path}");
            }
        }

        public async void DirectoryDialogOpenAction() {
            OpenFolderDialog dialog = new() { 
                Title = "请选择一个游戏目录"
            };
            var result = await dialog.ShowAsync(MainWindow.Instance);

            if (!string.IsNullOrEmpty(result) && result.IsDirectory()) {
                GameDirectorys.Add(result);
                App.LaunchInfoData.GameDirectorys.Add(result);
                CurrentGameDirectory = result;
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
    }
}
