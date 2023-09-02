using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Windows {
    public class MainWindowViewModel : ReactiveObject {
        public MainWindowViewModel() {
            this.PropertyChanged += OnPropertyChanged;
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
