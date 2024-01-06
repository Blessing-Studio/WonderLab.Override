using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Classes.Models.Game;

namespace WonderLab.Classes.Models.ViewData {
    public partial class GameViewData(GameEntry data) : ViewDataBase<GameEntry>(data) {
        [RelayCommand]
        private void Select() {
            WeakReferenceMessenger.Default.Send(this);
        }
    }
}
