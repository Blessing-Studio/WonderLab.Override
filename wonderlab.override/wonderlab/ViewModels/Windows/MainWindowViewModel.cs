using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using wonderlab.control.Controls.Dialog;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Windows
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel() {
            this.PropertyChanged += OnPropertyChanged;
        }

        [Reactive]
        public UserControl CurrentPage { get; set; } = new LaunchConfigPage();

        [Reactive]
        public double DownloadProgress { get; set; } = 0.0;

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }
        }
    }
}
