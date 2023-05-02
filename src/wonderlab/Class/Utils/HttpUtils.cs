using Avalonia.Media.Imaging;
using MinecraftLaunch.Modules.Installer;
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
using Tmds.DBus;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils
{
    public static class HttpUtils {
        const string MojangNewsAPI = "https://launchercontent.mojang.com/news.json";
        const string HitokotoAPI = "https://v1.hitokoto.cn/";

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

        public static async ValueTask<HitokotoModel> GetHitokotoTextAsync() { 
            var result = new HitokotoModel();

            try {
                var json = await (await HttpWrapper.HttpGetAsync(HitokotoAPI)).Content.ReadAsStringAsync();
                result = json.ToJsonEntity<HitokotoModel>();
            }
            catch (Exception ex) {
                Trace.WriteLine($"[信息] 异常名 {ex.GetType().Name}");
                Trace.WriteLine($"[信息] 异常信息 {ex.Message}");

                $"无法获取到一言，可能是您的网络出现了小问题，异常信息：{ex.Message}".ShowMessage();
            }

            return result;
        }

        public static async ValueTask<string> GetLatestGameCoreAsync() {
            var result = await GameCoreInstaller.GetGameCoresAsync();
            return result.Latest.Last().Value;
        }

        public static async ValueTask<Bitmap> GetWebBitmapAsync(string url) { 
            return await Task.Run(async () => {
                var bytes = await HttpWrapper.HttpClient.GetByteArrayAsync(url);

                using var stream = new MemoryStream(bytes);
                return new Bitmap(stream);
            });
        }

        public static async ValueTask<bool> ConnectionTestAsync(string url) {
            try {
                var result = await Task.Run(async () => await HttpWrapper.HttpClient.GetAsync(url));
                return true;
            }            
            catch (Exception) {           
                GC.Collect();
                return false;
            }
        }
    }
}
