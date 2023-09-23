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

namespace wonderlab.ViewModels.Windows {
    public class MainWindowViewModel : ReactiveObject {
        public MainWindowViewModel() {
            JsonUtils.CreateLaunchInfoJson();
            JsonUtils.CreateLauncherInfoJson();

            this.PropertyChanged += OnPropertyChanged;
            var launcherData = GlobalResources.LauncherData;

            ThreadPool.QueueUserWorkItem(async x => {
                ThemeUtils.SetAccentColor(launcherData.AccentColor);
                IsLoadImageBackground = launcherData.BakgroundType is "图片背景";
                string imagePath = launcherData.ImagePath;

                if (IsLoadImageBackground && imagePath.IsFile()) {
                    ImageSource = new Bitmap(imagePath);
                }

                //Load Data
                await Task.Run(async () => {
                    CacheResources.Accounts = (await AccountUtils.GetAsync(true)
                         .ToListAsync())
                         .ToObservableCollection();
                });

                APIManager.Current = launcherData.CurrentDownloadAPI switch {
                    DownloadApiType.Bmcl => APIManager.Bmcl,
                    DownloadApiType.Mcbbs => APIManager.Mcbbs,
                    DownloadApiType.Mojang => APIManager.Mojang,
                    _ => APIManager.Mcbbs,
                };

                CacheResources.GetWebModpackInfoData();
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

        public string Version => $"{GlobalResources.LauncherData.IssuingBranch} {UpdateUtils.LocalVersion}";

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }
        }
    }
}
