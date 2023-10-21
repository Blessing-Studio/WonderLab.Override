using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utils;
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
            SavesUtil toolkit = new(GlobalResources.LaunchInfoData.GameDirectoryPath);
            var result = await toolkit.LoadAllAsync(core);

            if (result.HasValue()) {

            }
        }
    }
}
