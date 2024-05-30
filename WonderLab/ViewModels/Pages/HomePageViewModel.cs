using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Extensions;
using WonderLab.Services;
using WonderLab.Services.Game;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly LogService _logService;
    private readonly GameService _gameService;
    
    [ObservableProperty]
    private GameViewData activeGameEntry;
    
    public ObservableCollection<GameViewData> GameEntries { get; private set; }
    
    /// <inheritdoc />
    public HomePageViewModel(GameService gameService, LogService logService) {
        _logService = logService;
        _gameService = gameService;

        GameEntries = _gameService.GameEntries.ToObservableList();
        _ = Task.Run(async () => {
            await Task.Delay(1000);
            ActiveGameEntry = _gameService.ActiveGameEntry;
        });
    }

    partial void OnActiveGameEntryChanged(GameViewData value) {
        _gameService.ActivateGameViewEntry(value);
    }
}