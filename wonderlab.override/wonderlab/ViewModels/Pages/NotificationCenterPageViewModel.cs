using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.ViewData;

namespace wonderlab.ViewModels.Pages
{
    public class NotificationCenterPageViewModel : ReactiveObject
    {
        public NotificationCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            Notifications.CollectionChanged += Notifications_CollectionChanged;
        }

        private void Notifications_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {       
            if (Notifications.Count != 0) {            
                HasNotification = false;
            } else HasNotification = true;               
        }

        [Reactive]
        public ObservableCollection<NotificationViewData> Notifications { get; set; } = new();

        [Reactive]
        public bool HasNotification { get; set; } = true;

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            Trace.WriteLine($"[信息] 更改的属性为 {e.PropertyName}");
        }
    }
}
