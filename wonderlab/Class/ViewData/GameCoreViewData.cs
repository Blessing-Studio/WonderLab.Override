using MinecraftLaunch.Modules.Models.Launch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.ViewData {
    public class GameCoreViewData : ViewDataBase<GameCore> {
        public GameCoreViewData(GameCore data) : base(data) {
        }

        public void OpenFolderAction() {
            using var process = Process.Start(new ProcessStartInfo(Data.Root.FullName) {
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
