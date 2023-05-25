using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wonderlab.Class.ViewData {
    public class WonderServerViewData : ViewDataBase<WonderServerModel> {
        public WonderServerViewData(WonderServerModel data) : base(data) {
            //_ = GetServerInfoAction();
        }

        [Reactive]
        public ServerInfoModel ServerInfo { get; set; } = new();

        public async ValueTask GetServerInfoAction() {
            ServerInfo = await new ServerUtils(Data.ServerIp, (ushort)Data.ServerPort).GetServerInfoAsync();
        }
    }
}
