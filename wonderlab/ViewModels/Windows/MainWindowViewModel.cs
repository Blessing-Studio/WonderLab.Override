using Avalonia;
using Avalonia.Controls;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control.Controls.Dialog;
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

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }
        }
    }
}
