using MinecraftLaunch.Modules.Models.Launch;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using wonderlab.Class.Utils;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class GameCoreConfigPageViewModel : ViewModelBase {
        public GameCoreConfigPageViewModel(GameCore core) {
            PropertyChanged += OnPropertyChanged;

            Current = core;
            CurrentPage = new ModConfigPage(core);

            try {
                AsyncRunAction();
                ModLoaders = core.HasModLoader ? string.Join(",", core.ModLoaderInfos.Select(x => x.ModLoaderType)) : "Vanllia";
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
            var result = await GameCoreUtils.GetTotalSizeAsync(Current);
            GameCoreTotalSize = result;
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
    }
}
