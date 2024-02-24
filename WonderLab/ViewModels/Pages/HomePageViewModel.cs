using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Services;
using WonderLab.Services.Game;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    private readonly LogService _logService;
    private readonly GameService _gameService;
    
    [ObservableProperty]
    private GameViewData activeGameEntry;
    
    public ReadOnlyObservableCollection<GameViewData> GameEntries { get; private set; }
    
    /// <inheritdoc />
    public HomePageViewModel(GameService gameService, LogService logService) {
        _logService = logService;
        _gameService = gameService;
        
        GameEntries = _gameService.GameEntries;
        ActiveGameEntry = _gameService.ActiveGameEntry;
    }
}