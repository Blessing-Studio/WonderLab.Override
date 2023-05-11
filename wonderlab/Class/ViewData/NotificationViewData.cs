using Avalonia.Threading;
using MinecraftLaunch.Modules.Installer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using wonderlab.Class.Utils;
using wonderlab.Views.Pages;
using Timer = System.Timers.Timer;

namespace wonderlab.Class.ViewData
{
    public class NotificationViewData : ReactiveObject
    {
        [Reactive]
        public string Title { set; get; } = "A notification";

        [Reactive]
        public string RunTime { set; get; } = "00:00:00";

        [Reactive]
        public string Progress { set; get; } = "0%";

        [Reactive]
        public double ProgressOfBar { set; get; } = 0;

        [Reactive]
        public DateTime Time { set; get; }

        public Timer Timer { set; get; } = new (1000);

        public void TimerStart() {
            Timer.Elapsed += TimerElapsed;
            Time = DateTime.Now;
            Timer.Start();
        }

        public void TimerStop() {
            Timer.Stop();
            NotificationCenterPage.ViewModel.Notifications.Remove(this);
        }

        void TimerElapsed(object? sender, ElapsedEventArgs e) {       
            RunTime = (DateTime.Now - Time).ToString(@"hh\:mm\:ss");
        }
    }
}
