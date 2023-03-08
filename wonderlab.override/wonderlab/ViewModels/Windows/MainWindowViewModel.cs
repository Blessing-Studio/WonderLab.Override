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
        }

        [Reactive]
        public UserControl CurrentPage { get; set; } = new HomePage();

        [Reactive]
        public double DownloadProgress { get; set; } = 0.0;

        [Reactive]
        public ObservableCollection<GameCoreEmtity> GameCores { get; set; }

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[信息] 活动页面已改变");
            }

            if (e.PropertyName == nameof(GameCores)) {
                GameCores.Clear();

                var temp = GameCores.Where(x => {
                    x.Type = x.Type switch {
                        "snapshot" => "快照版",
                        "release" => "正式版",
                        "old_alpha" => "远古版",
                        _ => "Fuck"
                    };

                    return true;
                });

                foreach (var item in temp) {
                    await Task.Delay(50);
                    GameCores.Add(item);
                }
            }
        }
    }
}
