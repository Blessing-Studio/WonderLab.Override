using Avalonia.Media.Imaging;
using MinecraftLaunch.Modules.Enum;
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
using wonderlab.Class.AppData;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils {
    public static class HttpUtils {

        public static async ValueTask<IEnumerable<New>> GetMojangNewsAsync() {
            var result = new List<New>();

            try {
                var json = await (await HttpWrapper.HttpGetAsync(GlobalResources.MojangNewsApi)).Content.ReadAsStringAsync();
                result = json.ToJsonEntity<MojangNewsModel>().Entries;
                CacheResources.MojangNews = result;
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
                var json = await (await HttpWrapper.HttpGetAsync(GlobalResources.HitokotoApi)).Content.ReadAsStringAsync();
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
                GC.Collect();
                return true;
            }
            catch (Exception) {
                GC.Collect();
                return false;
            }
        }

        public static async ValueTask<bool> GetModLoadersFromMcVersionAsync(string id) {
            try {
                await Task.Run(async () => CacheResources.Forges.AddRange(await GetForgesAsync()));
                await Task.Run(async () => CacheResources.Quilts.AddRange(await GetQuiltsAsync()));
                await Task.Run(async () => CacheResources.Fabrics.AddRange(await GetFabricsAsync()));
                await Task.Run(async () => CacheResources.Optifines.AddRange(await GetOptifinesAsync()));
            }
            catch (Exception) {
                GC.Collect();
            }

            return true;
            async ValueTask<IEnumerable<ModLoaderModel>> GetForgesAsync() {
                var result = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(id)).Select(x => new ModLoaderModel() {
                    ModLoaderType = ModLoaderType.Forge,
                    ModLoaderBuild = x,
                    GameCoreVersion = x.McVersion,
                    Id = x.ForgeVersion,
                    Time = x.ModifiedTime
                });

                if (!result.Any()) {
                    return Array.Empty<ModLoaderModel>();
                }

                return result;
            }

            async ValueTask<IEnumerable<ModLoaderModel>> GetQuiltsAsync() {
                var result = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(id)).Select(x => new ModLoaderModel() {
                    ModLoaderType = ModLoaderType.Quilt,
                    GameCoreVersion = x.Intermediary.Version,
                    ModLoaderBuild = x,
                    Id = x.Loader.Version,
                    Time = DateTime.Now
                });

                if (!result.Any()) {
                    return Array.Empty<ModLoaderModel>();
                }

                return result;
            }

            async ValueTask<IEnumerable<ModLoaderModel>> GetFabricsAsync() {
                var result = (await FabricInstaller.GetFabricBuildsByVersionAsync(id)).Select(x => new ModLoaderModel() {
                    ModLoaderType = ModLoaderType.Quilt,
                    GameCoreVersion = x.Intermediary.Version,
                    ModLoaderBuild = x,
                    Id = x.Loader.Version,
                    Time = DateTime.Now
                });

                if (!result.Any()) {
                    return Array.Empty<ModLoaderModel>();
                }

                return result;
            }

            async ValueTask<IEnumerable<ModLoaderModel>> GetOptifinesAsync() {
                var result = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(id)).Select(x => new ModLoaderModel() {
                    ModLoaderType = ModLoaderType.OptiFine,
                    ModLoaderBuild = x,
                    GameCoreVersion = x.McVersion,
                    Id = x.Type,
                    Time = DateTime.Now
                });

                if (!result.Any()) {
                    return Array.Empty<ModLoaderModel>();
                }

                return result;
            }
        }
    }
}
