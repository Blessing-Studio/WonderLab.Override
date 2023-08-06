using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;

namespace wonderlab.ViewModels.Pages {
    public class SaveConfigPageViewModel : ViewModelBase {
        private GameCore Core { get; set; }

        public SaveConfigPageViewModel(GameCore core) {
            _ = GetSavesAsync(core);
        }

        

        public async ValueTask GetSavesAsync(GameCore core) {
            SavesToolkit toolkit = new(GlobalResources.LaunchInfoData.GameDirectoryPath);
            var result = await toolkit.LoadAllAsync(core);

            if (result.HasValue()) {

            }
        }
    }
}
