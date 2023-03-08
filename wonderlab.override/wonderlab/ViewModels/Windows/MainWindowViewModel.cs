using Avalonia;
using Avalonia.Controls;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            GetGameCoresAction();
        }

        [Reactive]
        public UserControl CurrentPage { get; set; } = new HomePage();

        [Reactive]
        public double DownloadProgress { get; set; } = 0.0;

        [Reactive]
        public ObservableCollection<GameCoreEmtity> GameCores { get; set; } = new();

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }

            if (e.PropertyName == nameof(GameCores)) {
            }
        }

        public async void GetGameCoresAction() {
            var res = await GameCoreInstaller.GetGameCoresAsync();
            GameCores.Clear();
            
            var temp = res.Cores.Where(x => {
                x.Type = x.Type switch {
                    "snapshot" => "快照版本",
                    "release" => "正式版本",
                    "old_alpha" => "远古版本",
                    "old_beta" => "远古版本",
                    _ => "Fuck"
                } + $" {x.ReleaseTime.ToString(@"yyyy\-MM\-dd hh\:mm")}";
            
                return true;
            });
            
            foreach (var item in temp) {
                await Task.Delay(20);
                GameCores.Add(item);
            }
        }
    }
}
