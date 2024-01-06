using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Classes.Models.Game;

namespace WonderLab.Classes.Models.ViewData {
    public partial class GameViewData : ViewDataBase<GameEntry> {
        public GameViewData(GameEntry data) : base(data) {}

        [RelayCommand]
        private void Select() {
            WeakReferenceMessenger.Default.Send(this);
        }
    }
}
