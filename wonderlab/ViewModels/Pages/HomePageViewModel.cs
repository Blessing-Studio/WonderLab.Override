using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Models.Launch;
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
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab.ViewModels.Pages
{
    public class HomePageViewModel : ReactiveObject
    {
        public HomePageViewModel() {
            this.PropertyChanged += OnPropertyChanged;
            HasGameCore = GameCores.Any() ? 0 : 1;
        }

        [Reactive]
        public string SearchCondition { get; set; }

        [Reactive]
        public double SearchSuccess { get; set; } = 0;

        [Reactive]
        public double HasGameCore { get; set; } = 0;

        [Reactive]
        public GameCore SelectGameCore { get; set; }

        [Reactive]
        public ObservableCollection<GameCore> GameCores { get; set; } = new();

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(SearchCondition)) {
                SeachGameCore(SearchCondition);
            }

            if (e.PropertyName is nameof(SelectGameCore) && SelectGameCore != null) {
                App.LaunchInfoData.SelectGameCore = SelectGameCore.Id!;
            }
        }

        public async void SeachGameCore(string text) { 
            GameCores.Clear();
            GameCores = (await GameCoreUtils.SearchGameCoreAsync(new LaunchInfoDataModel().GameDirectoryPath, text))
                .Distinct().ToObservableCollection();

            if (!GameCores.Any()) {
                SearchSuccess = 1;
            }
            else SearchSuccess = 0;            
        }

        public async void GetGameCoresAction() {
            GameCores.Clear();
            var cores = await GameCoreUtils.GetLocalGameCores(new LaunchInfoDataModel().GameDirectoryPath);
            HasGameCore = cores.Any() ? 0 : 1;

            foreach (var i in cores) {
                await Task.Delay(20);    
                GameCores.Add(i);
            }
        }

        public async void LaunchTaskAction() {
            JavaMinecraftLauncher launcher = new(new(), App.LaunchInfoData.GameDirectoryPath, true);

            using var gameProcess = await launcher.LaunchTaskAsync(App.LaunchInfoData.SelectGameCore);            
        }
    }
}
