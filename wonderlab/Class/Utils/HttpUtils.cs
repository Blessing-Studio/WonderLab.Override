using Avalonia.Media.Imaging;
using Flurl.Http;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Models;
using wonderlab.Views.Pages;
using MinecraftLaunch.Modules.Utils;
using MinecraftLaunch.Modules.Models.Install;
using wonderlab.control;
using MinecraftLaunch.Modules.Models.Http;
using DynamicData;

namespace wonderlab.Class.Utils {
    public static class HttpUtils {
        public static async ValueTask<IEnumerable<New>> GetMojangNewsAsync() {
            var result = new List<New>();

            try {
                var json = await (await GlobalResources.MojangNewsApi.GetAsync())
                    .GetStringAsync();

                result = json.ToJsonEntity<MojangNewsModel>()
                    .Entries;

                CacheResources.MojangNews = result;
            }
            catch (Exception ex) {
                ex.ShowLog(LogLevel.Error);
                $"无法获取到新闻，可能是您的网络出现了小问题，异常信息：{ex.Message}"
                    .ShowMessage();
            }

            return result;
        }

        public static async ValueTask<HitokotoModel> GetHitokotoTextAsync() {
            var result = new HitokotoModel();

            try {
                var json = await (await GlobalResources.HitokotoApi.GetAsync())
                    .GetStringAsync();

                result = JsonSerializer.Deserialize<HitokotoModel>(json);
            }
            catch (Exception ex) {
                ex.ShowLog(LogLevel.Error);
                $"无法获取到一言，可能是您的网络出现了小问题，异常信息：{ex.Message}".ShowMessage();
            }

            return result;
        }

        public static async ValueTask<IEnumerable<GameCoreEmtity>> GetGameCoresAsync() {
            if (!CacheResources.GameCores.Any()) {
                var cores = await Task.Run(async () => await GameCoreInstaller.GetGameCoresAsync());
                var result = cores.Cores.Where(x => {
                    x.Type = x.Type switch {
                        "snapshot" => "快照版本",
                        "release" => "正式版本",
                        "old_alpha" => "远古版本",
                        "old_beta" => "远古版本",
                        _ => "正式版本"
                    } + $" {x.ReleaseTime.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                    return true;
                }).AsEnumerable();

                CacheResources.GameCores.AddRange(result);
                return result;
            }

            return CacheResources.GameCores;
        }

        public static async ValueTask<string> GetLatestGameCoreAsync() {
            var result = await GetGameCoresAsync();
            return result.FirstOrDefault().Id;
        }

        public static async ValueTask<Bitmap> GetWebBitmapAsync(string url) {
            try {
                return await Task.Run(async () => {
                    var bytes = await url.GetBytesAsync();

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
                await Task.Run(async () => (await url.GetAsync()).Dispose());
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
            var viewModel = DownCenterPage.ViewModel;
            var vm = InstallerPage.ViewModel;

            try {
                vm.IsQuiltLoaded = false;
                vm.IsFabricLoaded = false;
                vm.IsForgeLoaded = false;
                vm.IsNeoForgeLoaded = false;
                vm.IsQuiltLoaded = false;

                viewModel.Optifines.Clear();
                viewModel.NeoForges.Clear();
                viewModel.Fabrics.Clear();
                viewModel.Quilts.Clear();
                viewModel.Forges.Clear();

                await Task.Run(async() => {
                    await Task.Run(GetFabricsAsync);
                    await Task.Run(GetQuiltsAsync);
                    await Task.Run(GetOptifinesAsync);
                    await Task.Run(GetForgesAsync);
                    await Task.Run(GetNeoForgesAsync);
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
                    vm.IsForgeLoaded = true;
                    CacheResources.Forges.AddRange(result);
                    viewModel.Forges.AddRange(result);
                });
            }

            async ValueTask GetNeoForgesAsync() {
                await Task.Run(async () => {
                    var result = (await NeoForgeInstaller.GetNeoForgesOfVersionAsync(id).ToListAsync()).Select(x => new ModLoaderModel() {
                        ModLoaderType = ModLoaderType.NeoForged,
                        ModLoaderBuild = x,
                        GameCoreVersion = x.McVersion,
                        Id = x.NeoForgeVersion,
                        Time = DateTime.Now
                    });

                    if (!result.Any()) {
                        result = Array.Empty<ModLoaderModel>();
                    }

                    "NeoForge 加载完毕".ShowLog();
                    vm.IsNeoForgeLoaded = true;
                    CacheResources.NeoForges.AddRange(result);
                    viewModel.NeoForges.AddRange(result);
                });
            }

            async ValueTask GetQuiltsAsync() {
                await Task.Run(async () => {
                    if (id.Split('.').ElementAtOrDefault(1)?.ToInt32() < 14) {
                        $"Mc 版本 {id} 无可用的 Quilt".ShowLog();
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
                        vm.IsQuiltLoaded = true;
                        CacheResources.Quilts.AddRange(result);
                        viewModel.Quilts.AddRange(result);
                    }
                });
            }

            async ValueTask GetFabricsAsync() {
                await Task.Run(async () => {
                    if (id.Split('.').ElementAtOrDefault(1)?.ToInt32() < 14) {
                        $"Mc 版本 {id} 无可用的 Fabric".ShowLog();
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
                        vm.IsFabricLoaded = true;
                        CacheResources.Fabrics.AddRange(result);
                        viewModel.Fabrics.AddRange(result);
                    }
                });
            }

            async ValueTask GetOptifinesAsync() {
                await Task.Run(async () => {
                    var result = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(id)).Select(x => new ModLoaderModel() {
                        ModLoaderType = ModLoaderType.OptiFine,
                        ModLoaderBuild = x,
                        GameCoreVersion = x.McVersion,
                        Id = $"{x.Type}_{x.Patch}",
                        Time = DateTime.Now
                    });

                    if (!result.Any()) {
                        result = Array.Empty<ModLoaderModel>();
                    }

                    "Optifine 加载完毕".ShowLog();
                    vm.IsOptifineLoaded = true;
                    CacheResources.Optifines.AddRange(result);
                    viewModel.Optifines.AddRange(result);
                });
            }
        }

        public static async ValueTask<(ArticleJsonEntity, ArticleJsonEntity, IEnumerable<ArticleJsonEntity>)> GetMcVersionUpdatesAsync() {
            var result = (await McNewsUtil.GetMcVersionUpdatesAsync())
                .Where(x => x.PrimaryCategory is "News");

            return new(result.ElementAtOrDefault(0), result.ElementAtOrDefault(1), result);
        }
    }
}
