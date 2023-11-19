using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WonderLab.Classes.Handlers;
using WonderLab.Classes.Models;

namespace WonderLab.Classes.Managers {
    public class ConfigDataManager {
        private ConfigDataHandler _handler;

        public ConfigDataManager(ConfigDataHandler handler) {
            _handler = handler;
            Init();
        }

        public void Init() {
            Config = _handler.ConfigDataModel;
        }

        public ConfigDataModel Config { get; set; }
    }
}
