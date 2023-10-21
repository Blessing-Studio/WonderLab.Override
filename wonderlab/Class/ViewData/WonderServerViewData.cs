using Avalonia.Media;
using Avalonia.Media.Imaging;
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

        [Reactive]
        public IImage Icon { get; set; } = BitmapUtils.GetIconBitmap("defaultavatar.jpg");
             
        [Reactive]
        public bool IsImageLoading { get; set; }

        public async ValueTask GetServerInfoAction() {
            IsImageLoading = true;
            ServerInfo = await new ServerUtils(Data.ServerIp, (ushort)Data.ServerPort).GetServerInfoAsync();

            if (!ServerInfo.IsNull()) {
                var stream = ServerInfo.Response.Icon?.Replace("data:image/png;base64,", string.Empty)?.ToMemoryStream();

                if (!stream!.IsNull()) {
                    Icon = new Bitmap(stream);
                    stream.Dispose();
                }
            }

            IsImageLoading = false;
        }
    }
}
