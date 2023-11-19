using System;
using ReactiveUI;
using System.Windows.Input;
using System.Threading.Tasks;
using WonderLab.Views.Pages;
using ReactiveUI.Fody.Helpers;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Windows;
using WonderLab.Classes.Managers;
using WonderLab.Views.Pages.ControlCenter;
using WonderLab.Classes.Models.Tasks;

namespace WonderLab.ViewModels.Pages {
    public class HomePageViewModel : ViewModelBase {
        [Reactive]
        public bool IsOpenGameCoreBar { get; set; } = false;

        [Reactive]
        public double GameCoreBarHeight { get; set; } = 85;

        [Reactive]
        public double GameCoreBarWidth { get; set; } = 155;

        [Reactive]
        public double GameCoreListOpacity { get; set; } = 0;

        [Reactive]
        public double OtherControlOpacity { get; set; } = 1;

        [Reactive]
        public double ControlCenterBarWidth { get; set; } = 175;

        public object TaskCenterCardContent => App.ServiceProvider
            .GetRequiredService<TaskCenterPage>();

        public ICommand OpenGameCoreBarCommand 
            => ReactiveCommand.Create(OpenGameCoreBar);

        public ICommand CloseGameCoreBarCommand
            => ReactiveCommand.Create(CloseGameCoreBar);

        public ICommand OpenControlCenterCommand
            => ReactiveCommand.Create(ControlControlCenterBar);

        public async void OpenGameCoreBar() {
            var homePage = App.ServiceProvider
                .GetRequiredService<HomePage>();

            double height = homePage.Bounds.Height - 56,
                width = homePage.Bounds.Width;

            GameCoreBarWidth = width;
            GameCoreBarHeight = height;
            IsOpenGameCoreBar = !IsOpenGameCoreBar;            
            await Task.Delay(300).ContinueWith(x => GameCoreListOpacity = 1);

            homePage.WhenAnyValue(x => x.Bounds.Height, x => x.Bounds.Width)
            .Subscribe(x => {
                if (IsOpenGameCoreBar) {
                    GameCoreBarWidth = homePage.Bounds.Width;
                    GameCoreBarHeight = homePage.Bounds.Height - 56;
                }
            });
        }

        public void CloseGameCoreBar() {
            GameCoreListOpacity = 0;
            GameCoreBarHeight = 85;
            GameCoreBarWidth = 155;
            IsOpenGameCoreBar = !IsOpenGameCoreBar;
        }

        public void ControlControlCenterBar() {
            var homePage = App.ServiceProvider
                .GetRequiredService<HomePage>();

            var vm = App.ServiceProvider
                .GetRequiredService<MainWindowViewModel>();

            if (OtherControlOpacity is 0) {
                vm.IsFullScreen = false;
                ControlCenterBarWidth = 175;
                OtherControlOpacity = 1;
            } else {
                OpenControlCenter(homePage);
            }

            homePage.WhenAnyValue(x => x.Bounds.Height, x => x.Bounds.Width)
                .Subscribe(x => {
                    if (OtherControlOpacity == 0) {
                        ControlCenterBarWidth = homePage.Bounds.Width;
                    }
                });
        }

        public void test() {
            DownloadTask task = new("https://github.moeyy.xyz/https://github.com/Blessing-Studio/WonderLab.Override/releases/download/AutoBuild_master/wonderlab.1.2.8.2.win-x64.zip",
                new(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), "javaw.exe");
            var manager = App.ServiceProvider.GetRequiredService<TaskManager>();
            manager.QueueJob(task);
        }

        private void OpenControlCenter(HomePage homePage) {
            var vm = App.ServiceProvider
                .GetRequiredService<MainWindowViewModel>();

            vm.IsFullScreen = true;
            OtherControlOpacity = 0;
            ControlCenterBarWidth = homePage.Bounds.Width;
        }
    }
}
