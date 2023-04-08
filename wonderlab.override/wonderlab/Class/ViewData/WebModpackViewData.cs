using Avalonia.Media.Imaging;
using Natsurainko.Toolkits.Network;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab.Class.ViewData
{
    public class WebModpackViewData : ViewDataBase<WebModpackModel>
    {
        public WebModpackViewData(WebModpackModel data) : base(data) {
            GetModpackIconAsync();
        }

        [Reactive]
        public Bitmap Icon { get; set; }

        public async void GetModpackIconAsync() {
            await Task.Run(async () => {
                try {
                    var result = await (await HttpWrapper.HttpGetAsync(Data.IconUrl)).Content.ReadAsByteArrayAsync();
                    Icon = new(new MemoryStream(result));
                }
                catch (Exception ex) {        
                    ex.Message.ShowMessage("Error");
                }
            });
        }
    }
}
