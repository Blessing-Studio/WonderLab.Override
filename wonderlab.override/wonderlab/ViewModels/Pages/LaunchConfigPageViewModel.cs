using Avalonia.Controls;
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
                ThreadPool.QueueUserWorkItem(x =>
                {
                    Javas = App.LaunchInfoData.JavaRuntimes.Select(x => x.ToJava()).ToObservableCollection();
                    CurrentJava = App.LaunchInfoData.JavaRuntimePath.ToJava();
                });
            }

            if (App.LaunchInfoData.GameDirectorys.Any()) {
                CurrentGameDirectory = App.LaunchInfoData.GameDirectoryPath;
                GameDirectorys = App.LaunchInfoData.GameDirectorys.ToObservableCollection();
            }
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Javas)) {
                App.LaunchInfoData.JavaRuntimes = Javas.Select(x => x.JavaPath).ToList();
            }

            if(e.PropertyName == nameof(CurrentJava)) { 
                App.LaunchInfoData.JavaRuntimePath = CurrentJava.JavaPath;
            }

            if (e.PropertyName == nameof(CurrentGameDirectory)) {
                App.LaunchInfoData.GameDirectoryPath = CurrentGameDirectory;
            }
        }

        [Reactive]
        public bool IsLoadJavaFinish { get; set; } = true;

        [Reactive]
        public bool IsLoadJavaNow { get; set; } = false;

        [Reactive]
        public JavaInfo CurrentJava { get; set; }

        [Reactive]
        public string CurrentGameDirectory { get; set; }

        [Reactive]
        public ObservableCollection<JavaInfo> Javas { get; set; } = new();

        [Reactive]
        public ObservableCollection<string> GameDirectorys { get; set; } = new();

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
            var javaInfo = JavaToolkit.GetJavaInfo(Path.Combine(new FileInfo(path).Directory.FullName, "java.exe"));
            Javas.Add(javaInfo);
            App.LaunchInfoData.JavaRuntimes.Add(path);
            CurrentJava = javaInfo;
            Trace.WriteLine($"[信息] 这是第 {Javas.Count} 找到的 Java 运行时，完整路径为 {path}");
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
    }
}
