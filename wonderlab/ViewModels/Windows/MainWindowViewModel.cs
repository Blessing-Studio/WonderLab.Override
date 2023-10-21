using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;
using wonderlab.Views.Pages;
using MinecraftLaunch.Modules.Utils;
using MinecraftLaunch.Modules.Models.Download;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using System;
using wonderlab.Views.Dialogs;
using DialogHostAvalonia;

namespace wonderlab.ViewModels.Windows {
    public class MainWindowViewModel : ViewModelBase {
        public MainWindowViewModel() {
            this.PropertyChanged += OnPropertyChanged;

            ThreadPool.QueueUserWorkItem(async x => {
                var launcherData = GlobalResources.LauncherData;

                IsLoadImageBackground = launcherData.BakgroundType is "图片背景";
                IsLoadColorBackground = launcherData.BakgroundType is "主题色背景";
                IsLoadAcrylicBackground = launcherData.BakgroundType is "亚克力背景";
                string imagePath = launcherData.ImagePath;
                
                if (IsLoadImageBackground && imagePath.IsFile()) {
                    ImageSource = new Bitmap(imagePath);
                }

                Dispatcher.UIThread.Post(() => {
                    List<WindowTransparencyLevel> effectBackground = new();
                    if (IsLoadAcrylicBackground) {
                        effectBackground.Add(WindowTransparencyLevel.AcrylicBlur);
                        App.CurrentWindow.TransparencyLevelHint = effectBackground;
                    }

                    bool isLoadMica = launcherData.BakgroundType.Contains("云母背景");
                    if (isLoadMica) {
                        effectBackground.Add(WindowTransparencyLevel.Mica);
                        App.CurrentWindow.TransparencyLevelHint = effectBackground;
                    }
                });

                APIManager.Current = launcherData.CurrentDownloadAPI switch {
                    DownloadApiType.Bmcl => APIManager.Bmcl,
                    DownloadApiType.Mcbbs => APIManager.Mcbbs,
                    DownloadApiType.Mojang => APIManager.Mojang,
                    _ => APIManager.Mcbbs,
                };


                var result = await UpdateUtils.GetLatestVersionInfoAsync();
                if (UpdateUtils.Check(result)) {
                    string time = DateTime.Parse(result["time"].GetValue<string>())
                        .ToString("yyyy-MM-dd HH:MM:ff");

                    Dispatcher.Post(async () => {
                        UpdateDialogContent content = new(result,
                            string.Join("\n", result["messages"].AsArray()),
                            $"于 {time} 发布，发行分支：{GlobalResources.LauncherData.IssuingBranch}");

                        await Task.Delay(500);
                        await DialogHost.Show(content, "dialogHost");
                    });
                }
            });
        }

        [Reactive]
        public Bitmap ImageSource { get; set; }

        [Reactive]
        public UserControl CurrentPage { get; set; } = new HomePage();

        [Reactive]
        public string NotificationCountText { get; set; } = "通知中心";

        [Reactive]
        public bool HasNotification { get; set; } = false;

        [Reactive]
        public bool IsLoadImageBackground { get; set; } = false;

        [Reactive]
        public bool IsLoadColorBackground { get; set; } = false;
        
        [Reactive]
        public bool IsLoadAcrylicBackground { get; set; } = false;

        public string Version => $"{GlobalResources.LauncherData.IssuingBranch} {UpdateUtils.LocalVersion}";

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }
        }
    }
}
