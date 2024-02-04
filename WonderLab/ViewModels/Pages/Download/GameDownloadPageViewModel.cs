using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Classes.Models.Install;
using MinecraftLaunch.Components.Installer;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WonderLab.Classes.Utilities;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class GameDownloadPageViewModel : ViewModelBase
{
    public GameDownloadPageViewModel()
    {
        Init();
    }

    [ObservableProperty]
    public ObservableCollection<VersionManifestEntry> gameCores = [];

    private async void Init()
    {
        await Task.Delay(1000);
        await Task.Run(async () =>
        {
            return await VanlliaInstaller.EnumerableGameCoreAsync();
        }).ContinueWith(async task =>
        {
            var cores = (await task);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                GameCores.Load(cores);
            });
        });
    }
}