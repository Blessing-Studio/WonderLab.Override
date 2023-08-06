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
using System.Text.Json;
using System.Threading.Tasks;
using Tmds.DBus;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Models;
using wonderlab.Views.Pages;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
                ex.ShowLog(LogLevel.Error);
                $"无法获取到新闻，可能是您的网络出现了小问题，异常信息：{ex.Message}".ShowMessage();
            }

            return result;
        }

        public static async ValueTask<HitokotoModel> GetHitokotoTextAsync() {
            var result = new HitokotoModel();

            try {
                var json = await (await HttpWrapper.HttpGetAsync(GlobalResources.HitokotoApi)).Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<HitokotoModel>(json);
            }
            catch (Exception ex) {
                ex.ShowLog(LogLevel.Error);
                $"无法获取到一言，可能是您的网络出现了小问题，异常信息：{ex.Message}".ShowMessage();
            }

            return result;
        }

        public static async ValueTask<string> GetLatestGameCoreAsync() {
            var result = await GameCoreInstaller.GetGameCoresAsync();
            return result.Latest.Last().Value;
        }

        public static async ValueTask<Bitmap> GetWebBitmapAsync(string url) {
            try {
                return await Task.Run(async () => {
                    var bytes = await HttpWrapper.HttpClient.GetByteArrayAsync(url);

                    using var stream = new MemoryStream(bytes);
                    return new Bitmap(stream);
                });
            }
            catch (Exception) {
                "拉取图片时遭遇了异常".ShowMessage("错误");
            }

            return null!;
        }

        public static async ValueTask<bool> ConnectionTestAsync(string url) {
            try {
                var result = await Task.Run(async () => await HttpWrapper.HttpClient.GetAsync(url));
                GC.Collect();
                return true;
            }
            catch (Exception ex) {
                GC.Collect();
                ex.ShowLog(LogLevel.Error);
                return false;
            }
        }

        public static async ValueTask<bool> GetModLoadersFromMcVersionAsync(string id) {
            try {
                await Task.Run(async() => {
                    await Task.Run(GetFabricsAsync);
                    await Task.Run(GetQuiltsAsync);
                    await Task.Run(GetOptifinesAsync);
                    await Task.Run(GetForgesAsync);
                });
            }
            catch (Exception ex) {
                ex.ShowLog(LogLevel.Error);
                GC.Collect();
            }
            finally {
                GC.Collect();
            }

            return true;

            async ValueTask GetForgesAsync() {
                DialogPage.ViewModel.IsQuiltLoaded = false;
                await Task.Run(async () => {
                    var result = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(id)).Select(x => new ModLoaderModel() {
                        ModLoaderType = ModLoaderType.Forge,
                        ModLoaderBuild = x,
                        GameCoreVersion = x.McVersion,
                        Id = x.ForgeVersion,
                        Time = x.ModifiedTime
                    });

                    if (!result.Any()) {
                        result = Array.Empty<ModLoaderModel>();
                    }

                    "Forge 加载完毕".ShowLog();       
                    CacheResources.Forges.AddRange(result);
                    DialogPage.ViewModel.IsForgeLoaded = result.Any();
                });
            }

            async ValueTask GetQuiltsAsync() {
                await Task.Run(async () => {
                    DialogPage.ViewModel.IsQuiltLoaded = false;
                    if (id.Split('.').GetValueInArray(1).ToInt32() < 14) {
                        $"Mc 版本 {id} 无可用的 Quilt".ShowLog();
                        DialogPage.ViewModel.IsQuiltLoaded = false;
                    } else {
                        var result = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(id)).Select(x => new ModLoaderModel() {
                            ModLoaderType = ModLoaderType.Quilt,
                            GameCoreVersion = x.Intermediary.Version,
                            ModLoaderBuild = x,
                            Id = x.Loader.Version,
                            Time = DateTime.Now
                        });

                        if (!result.Any()) {
                            result = Array.Empty<ModLoaderModel>();
                        }

                        "Quilt 加载完毕".ShowLog();
                        CacheResources.Quilts.AddRange(result);
                        DialogPage.ViewModel.IsQuiltLoaded = result.Any();
                    }
                });
            }

            async ValueTask GetFabricsAsync() {
                DialogPage.ViewModel.IsFabricLoaded = false;
                await Task.Run(async () => {
                    if (id.Split('.').GetValueInArray(1).ToInt32() < 14) {
                        $"Mc 版本 {id} 无可用的 Fabric".ShowLog();
                        DialogPage.ViewModel.IsFabricLoaded = false;
                    } else {
                        var result = (await FabricInstaller.GetFabricBuildsByVersionAsync(id)).Select(x => new ModLoaderModel() {
                            ModLoaderType = ModLoaderType.Fabric,
                            GameCoreVersion = x.Intermediary.Version,
                            ModLoaderBuild = x,
                            Id = x.Loader.Version,
                            Time = DateTime.Now
                        });

                        if (!result.Any()) {
                            result = Array.Empty<ModLoaderModel>();
                        }

                        "Fabric 加载完毕".ShowLog();
                        CacheResources.Fabrics.AddRange(result);
                        DialogPage.ViewModel.IsFabricLoaded = result.Any();
                    }
                });
            }

            async ValueTask GetOptifinesAsync() {
                DialogPage.ViewModel.IsOptifineLoaded = false;
                await Task.Run(async () => {
                    var result = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(id)).Select(x => new ModLoaderModel() {
                        ModLoaderType = ModLoaderType.OptiFine,
                        ModLoaderBuild = x,
                        GameCoreVersion = x.McVersion,
                        Id = x.Type,
                        Time = DateTime.Now
                    });

                    if (!result.Any()) {
                        result = Array.Empty<ModLoaderModel>();
                    }

                    "Optifine 加载完毕".ShowLog();
                    CacheResources.Optifines.AddRange(result);
                    DialogPage.ViewModel.IsOptifineLoaded = result.Any();
                });
            }
        }
    }
}
