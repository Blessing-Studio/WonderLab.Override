using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Classes.Models.Game;

namespace WonderLab.Classes.Datas.ViewData;

public sealed class GameViewData(GameEntry gameEntry) : ObservableObject {
    public GameEntry Entry => gameEntry;
    
    public override bool Equals(object? obj) {
        var gameViewData = obj as GameViewData;

        return gameViewData!.Entry.Id == Entry.Id &&
               gameViewData.Entry.GameFolderPath == Entry.GameFolderPath;
    }

    public override string ToString() {
        if (this is null || Entry is null) {
            return "6";
        }

        return Entry.Id;
    }

    public override int GetHashCode() {
        return Entry.Id.GetHashCode();
    }
}