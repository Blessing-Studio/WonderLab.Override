using WonderLab.Classes.Models;
using WonderLab.Classes.Handlers;
using MinecraftLaunch.Modules.Models.Launch;

namespace WonderLab.Classes.Managers {
    /// <summary>
    /// 数据管理类，包含了持久化数据和运行时数据
    /// </summary>
    public class DataManager {
        private ConfigDataHandler _handler;

        public DataManager(ConfigDataHandler handler) {
            _handler = handler;
            Init();
        }

        public void Init() {
            Config = _handler.ConfigDataModel ?? new();            
        }

        public ConfigDataModel Config { get; set; }    
    }
}