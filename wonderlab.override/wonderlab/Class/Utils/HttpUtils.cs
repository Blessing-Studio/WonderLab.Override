using Avalonia.Media.Imaging;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils
{
    public class HttpUtils {
        const string MojangNewsAPI = "https://launchercontent.mojang.com/news.json";

        public static async ValueTask<IEnumerable<New>> GetMojangNewsAsync() {
            var result = new List<New>();

			try {			
                var json = await (await HttpWrapper.HttpGetAsync(MojangNewsAPI)).Content.ReadAsStringAsync();
                result = json.ToJsonEntity<MojangNewsModel>().Entries;
            }
            catch (Exception ex) {
                Trace.WriteLine($"[信息] 异常名 {ex.GetType().Name}");
                Trace.WriteLine($"[信息] 异常信息 {ex.Message}");

                $"无法获取到新闻，可能是您的网络出现了小问题，异常信息：{ex.Message}".ShowMessage();
			}

            return result;
        }

        public static async ValueTask<Bitmap> GetWebBitmapAsync(string url) { 
            return await Task.Run(async () => {  
                var bytes = await (await HttpWrapper.HttpGetAsync(url)).Content.ReadAsByteArrayAsync();
                return new Bitmap(new MemoryStream(bytes));
            });
        }
    }
}
