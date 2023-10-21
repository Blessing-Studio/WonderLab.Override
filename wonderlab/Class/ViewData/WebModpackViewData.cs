using Avalonia.Media.Imaging;
using Flurl.Http;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab.Class.ViewData {
    public class WebModpackViewData : ViewDataBase<WebModpackModel>
    {
        public WebModpackViewData(WebModpackModel data) : base(data) {
            _ = GetModpackIconAsync();
        }

        [Reactive]
        public Bitmap Icon { get; set; }

        [Reactive]
        public bool IsLoading { get; set; }

        public async ValueTask GetModpackIconAsync() {
            try {
                IsLoading = true;

                using var result = new MemoryStream(await Data.IconUrl.GetBytesAsync()) { Position = 0 };
                var cache = new Bitmap(result);
                Icon = cache.CreateScaledBitmap(new(Convert.ToInt32(cache.Size.Width / 4), Convert.ToInt32(cache.Size.Height / 4)));

                IsLoading = false;
            }
            catch (Exception ex) {        
                ex.Message.ShowMessage("Error");
            }
        }
    }
}
