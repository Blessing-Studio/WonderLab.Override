using Avalonia.Controls;
using DialogHostAvalonia;
using MinecraftLaunch.Modules.Models.Download;
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

namespace wonderlab.ViewModels.Windows {
    public class MainWindowViewModel : ReactiveObject {
        public MainWindowViewModel() {
            this.PropertyChanged += OnPropertyChanged;
            var launchData = GlobalResources.LauncherData;

            ThreadPool.QueueUserWorkItem(async x => {
                await Task.Run(async () => {
                    CacheResources.Accounts = (await AccountUtils.GetAsync(true)
                                                                 .ToListAsync())
                                                                 .ToObservableCollection();
                });

                if (launchData.CurrentDownloadAPI is DownloadApiType.Mcbbs) {
                    APIManager.Current = APIManager.Mcbbs;
                } else if (launchData.CurrentDownloadAPI is DownloadApiType.Bmcl) {
                    APIManager.Current = APIManager.Bmcl;
                } else if (launchData.CurrentDownloadAPI is DownloadApiType.Mojang) {
                    APIManager.Current = APIManager.Mojang;
                } else {
                    launchData.CurrentDownloadAPI = DownloadApiType.Mojang;
                }
            });
        }

        [Reactive]
        public UserControl CurrentPage { get; set; } = new HomePage();

        [Reactive]
        public string NotificationCountText { get; set; } = "通知中心";

        [Reactive]
        public bool HasNotification { get; set; } = false;

        public string Version => $"{GlobalResources.LauncherData.IssuingBranch} {UpdateUtils.LocalVersion}";

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }
        }
    }
}
