using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Launch;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;
using wonderlab.Views.Windows;

namespace wonderlab.ViewModels.Pages {
    public class ConsoleCenterPageViewModel : ViewModelBase {
        public ConsoleCenterPageViewModel() {
            GameProcesses = CacheResources.GameProcesses!.Select(x => x
                .CreateViewData<MinecraftLaunchResponse, MinecraftProcessViewData>())
                .ToObservableCollection();
        }

        [Reactive]
        public ObservableCollection<MinecraftProcessViewData> GameProcesses { get; set; } = new();

        [Reactive]
        public ConsolePage CurrentPage { get; set; }
    }
}
