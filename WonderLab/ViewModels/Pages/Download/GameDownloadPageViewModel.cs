using Avalonia.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Utilities;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Classes.Models.Install;

namespace WonderLab.ViewModels.Pages.Download {
    public partial class GameDownloadPageViewModel : ViewModelBase {
        public GameDownloadPageViewModel() { 
            Init();
        }

        [ObservableProperty]
        public ObservableCollection<VersionManifestEntry> gameCores = new();

        private async void Init() {
            await Task.Delay(1000);
            await Task.Run(async () => {
                return await VanlliaInstaller.EnumerableGameCoreAsync();
            }).ContinueWith(async task => {
                var cores = (await task);

                await Dispatcher.UIThread.InvokeAsync(() => {
                    GameCores.Load(cores);
                },DispatcherPriority.Render);
            });
        }
    }
}
