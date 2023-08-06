using Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.ViewData;
using wonderlab.Views.Windows;

namespace wonderlab.ViewModels.Pages {
    public class NotificationCenterPageViewModel : ViewModelBase {
        public NotificationCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            Notifications.CollectionChanged += Notifications_CollectionChanged;
        }

        private void Notifications_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            if (Notifications.Count != 0) {
                HasNotification = false;
                MainWindow.ViewModel.NotificationCountText = $"共有 {Notifications.Count} 个正在执行的任务";
                MainWindow.ViewModel.HasNotification = true;

            } else {
                HasNotification = true;
                MainWindow.ViewModel.HasNotification = false;
                MainWindow.ViewModel.NotificationCountText = $"通知中心";
            }
        }

        [Reactive]
        public ObservableCollection<NotificationViewData> Notifications { get; set; } = new();

        [Reactive]
        public bool HasNotification { get; set; } = true;

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            Trace.WriteLine($"[信息] 更改的属性为 {e.PropertyName}");
        }

        public void CloseLauncherAction() {
            App.CurrentWindow.Close();
        }
    }
}
