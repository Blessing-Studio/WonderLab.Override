using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using WonderLab.Classes.Datas.ViewData;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Resolver;

namespace WonderLab.Services.Game;

/// <summary>
/// 游戏实体 <see cref="GameEntry"/> 相关操作服务类
/// </summary>
public sealed class GameService {
    private readonly ILogger<GameService> _logger;
    private readonly SettingService _settingService;
    private readonly ObservableCollection<GameViewData> _gameEntries;
    
    public IGameResolver GameResolver { get; private set; }
    public GameViewData ActiveGameEntry { get; private set; }
    public ReadOnlyObservableCollection<GameViewData> GameEntries { get; }

    public GameService(SettingService settingService, ILogger<GameService> logger) {
        _logger = logger;
        _settingService = settingService;

        _gameEntries = [];
        GameEntries = new(_gameEntries);
        if (!string.IsNullOrEmpty(_settingService?.Data?.ActiveGameFolder)) {
            Initialize();
        }
    }

    private void Initialize() {
        _logger.LogInformation("开始初始化游戏实例服务");
        GameResolver = new GameResolver(_settingService?.Data?.ActiveGameFolder ?? "C:\\Users\\w\\Desktop\\temp\\.minecraft");
        RefreshGameViewEntry();
    }
    
    public void RefreshGameViewEntry() {
        _gameEntries.Clear();
        var gameViewEntries = GameResolver.GetGameEntitys()
            .Select(x => new GameViewData(x));
        if (gameViewEntries is null || !gameViewEntries.Any()) return;

        foreach (var gameViewEntry in gameViewEntries) {
            _gameEntries.Add(gameViewEntry);
        }

        if (!string.IsNullOrEmpty(_settingService?.Data?.ActiveGameId)) {
            var gameEntry = GameResolver.GetGameEntity(_settingService.Data.ActiveGameId);
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