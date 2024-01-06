using System.Collections.Generic;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Resolver;

namespace WonderLab.Classes.Managers;

public class GameCoreManager {
    private readonly GameResolver _gameResolver = new();
    
    public IEnumerable<GameEntry> GetGameEntries(string path) {
        _gameResolver.Root = new(path);
        return _gameResolver.GetGameEntitys();
    }

    public GameEntry GetGameEntry(string path, string id) {
        _gameResolver.Root = new(path);
        return _gameResolver.GetGameEntity(path);
    }
}