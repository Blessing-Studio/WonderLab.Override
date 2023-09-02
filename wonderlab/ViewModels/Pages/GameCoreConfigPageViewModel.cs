using MinecraftLaunch.Modules.Models.Launch;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class GameCoreConfigPageViewModel : ViewModelBase {
        private GameCoreViewData cache = null!;

        public GameCoreConfigPageViewModel(GameCoreViewData core) {
            PropertyChanged += OnPropertyChanged;

            cache = core;
            Current = core.Data;
            CurrentPage = new SingleGameCoreConfigPage(core);

            try {
                AsyncRunAction();
                ModLoaders = core.Data.HasModLoader ? string.Join(",", core.Data.ModLoaderInfos.Select(x => x.ModLoaderType)) : "Vanllia";
            }
            catch (Exception) {

            }
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {

        }

        [Reactive]
        public object CurrentPage { get; set; }

        [Reactive]
        public GameCore Current { get; set; }

        [Reactive]
        public string ModLoaders { get; set; }

        [Reactive]
        public string GameCoreTotalSize { get; set; } = "0.00 MB";

        public async void AsyncRunAction() {
            if (!Current.IsNull()) {
                var result = await GameCoreUtils.GetTotalSizeAsync(Current);
                GameCoreTotalSize = result;
            }
        }

        public override void GoBackAction() {
            new HomePage().Navigation();
            App.CurrentWindow.OpenTopBar();
        }

        public void OpenFolderAction() {
            Process.Start(new ProcessStartInfo(Current.Root!.FullName) {
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void GoModPackAction() {
            CurrentPage = new ModConfigPage(Current);
        }

        public void GoSharePackAction() {
            CurrentPage = new SharePackConfigPage();
        }

        public void GoResourePackAction() {
            CurrentPage = new ResourePackConfigPage(Current);
        }

        public void GoSaveAction() {
            CurrentPage = new SaveConfigPage();
        }

        public void GoSingleConfigPage() {
            CurrentPage = new SingleGameCoreConfigPage(cache);
        }
    }
}
