using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class GameCoreUtils {   
        public static async ValueTask<ObservableCollection<GameCore>> GetLocalGameCores(string root) { 
            var cores = await Task.Run(() => {
                return new GameCoreToolkit(root).GetGameCores();
            });

            return cores is null ? new() : cores.ToObservableCollection();
        }

        public static async ValueTask<ObservableCollection<GameCore>> SearchGameCoreAsync(string root,string text) { 
            var cores = await Task.Run(() => {
                try {
                    return new GameCoreToolkit(root).GameCoreScearh(text);
                }
                catch {}

                return null;
            });

            return cores is null ? new() : cores.ToObservableCollection();
        }

        public static string GetGameCoreVersionPath(GameCore core) {
            return Path.Combine(core.Root!.FullName, "versions", core.Id!);
        }
    }
}
