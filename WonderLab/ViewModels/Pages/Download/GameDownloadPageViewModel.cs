using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Classes.Utilities;

namespace WonderLab.ViewModels.Pages.Download {
    public partial class GameDownloadPageViewModel : ViewModelBase {
        public GameDownloadPageViewModel() { 
            Init();
        }

        [ObservableProperty]
        public ObservableCollection<GameCoreEmtity> gameCores = new();

        private async void Init() {
            await Task.Delay(1000);
            await Task.Run(async () => {
                return await GameCoreInstaller.GetGameCoresAsync();
            }).ContinueWith(async task => {
                var cores = (await task).Cores;

                await Dispatcher.UIThread.InvokeAsync(() => {
                    GameCores.Load(cores);
                },DispatcherPriority.Render);
            });
        }
    }
}
