using System.Collections.ObjectModel;
using System.Linq;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Resolver;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.Services.Game;

/// <summary>
/// 游戏实体 <see cref="GameEntry"/> 相关操作服务类
/// </summary>
public sealed class GameService {
    private IGameResolver _gameResolver;
    private readonly LogService _logService;
    private readonly SettingService _settingService;
    private readonly ObservableCollection<GameViewData> _gameEntries;
    
    public GameViewData ActiveGameEntry { get; private set; }
    public ReadOnlyObservableCollection<GameViewData> GameEntries { get; }

    public GameService(SettingService settingService, LogService logService) {
        _logService = logService;
        _settingService = settingService;
        _gameEntries = new();
        GameEntries = new(_gameEntries);
        if (!string.IsNullOrEmpty(_settingService?.Data?.ActiveGameFolder)) {
            Initialize();
        }
    }

    private void Initialize() {
        _logService.Info(nameof(GameService), "Start initializing this service");

        _gameResolver = new GameResolver(_settingService?.Data?.ActiveGameFolder ?? "C:\\Users\\w\\Desktop\\temp\\.minecraft");
        RefreshGameViewEntry();
    }
    
    public void RefreshGameViewEntry() {
        _gameEntries.Clear();
        var gameViewEntries = _gameResolver.GetGameEntitys()
            .Select(x => new GameViewData(x));
        if (gameViewEntries is null || !gameViewEntries.Any()) return;

        foreach (var gameViewEntry in gameViewEntries) {
            _gameEntries.Add(gameViewEntry);
        }

        if (!string.IsNullOrEmpty(_settingService?.Data?.ActiveGameId)) {
            var gameEntry = _gameResolver.GetGameEntity(_settingService.Data.ActiveGameId);
            if (gameEntry is not null) {
                var gameViewEntry = new GameViewData(gameEntry);
                ActivateGameViewEntry(gameViewEntry);
            } else Empty();
        }
        else {
            Empty();
        }

        if (_gameEntries.Any() && ActiveGameEntry is null) {
            ActivateGameViewEntry(_gameEntries.First());
        }

        void Empty() {
            ActiveGameEntry = null!;
            _settingService.Data.ActiveGameId = null!;
        }
    }
    
    public void ActivateGameViewEntry(GameViewData gameViewEntry) {
        if (ActiveGameEntry is null || !ActiveGameEntry.Equals(gameViewEntry)) {
            ActiveGameEntry = gameViewEntry;
            _settingService.Data.ActiveGameId = gameViewEntry.Entry.Id;
        }
    }
}