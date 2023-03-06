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
using wonderlab.Class.Interface;
using wonderlab.Class.Utils;

namespace wonderlab.Class.ViewData
{
    public class NotificationViewData : ReactiveObject, INotification
    {
        [Reactive]
        public string Title { set; get; } = "A notification";

        [Reactive]
        public string RunTime { set; get; } = "00:00:00";

        [Reactive]
        public string Progress { set; get; } = "0%";

        [Reactive]
        public double ProgressOfBar { set; get; } = 0;

        public DateTime Time { set; get; }


        public NotificationViewData() {
            PropertyChanged += OnPropertyChanged;
            Task.Run(Begin);
        }

        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            Trace.WriteLine($"[信息] 更改的属性为 {e.PropertyName}");
        }

        public async void Begin() {
            await GameInstall();
        }

        async ValueTask GameInstall() {
            GameCoreInstaller installer = new("C:\\Users\\w\\Desktop\\temp\\text\\.minecraft", "1.19.3");
            Title = $"游戏 {installer.Id} 安装任务";
            System.Timers.Timer timer = new(1000);
            timer.Elapsed += Timer_Elapsed;
            Time = DateTime.Now;
            timer.Start();

            installer.ProgressChanged += (_, x) =>
            {
                Thread.Sleep(0);
                var round = Math.Round(x.Progress * 100, 0);
                ProgressOfBar = x.Progress * 100;
                Progress = $"{round}%";
            };
            
            var res = await installer.InstallAsync();
            if (res.Success) {
                timer.Stop();
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    MainWindow.Instance.ShowInfoBar("信息", $"游戏核心 {res.GameCore.Id} 安装完成");
                });
            }
        }

        void Timer_Elapsed(object? sender, ElapsedEventArgs e) {       
            RunTime = (DateTime.Now - Time).ToString(@"hh\:mm\:ss");
        }
    }
}
