using Avalonia.OpenGL;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Splat.ModeDetection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils
{
    public static class GameCoreUtils {   
        public static async ValueTask<ObservableCollection<GameCore>> GetLocalGameCores(string root) { 
            var cores = await Task.Run(() => {
                return new GameCoreToolkit(root).GetGameCores();
            });

            return cores is null ? new() : cores.ToObservableCollection();
        }

        public static async ValueTask<ObservableCollection<GameCore>> SearchGameCoreAsync(string root, string text) { 
            var cores = await Task.Run(() => {
                try {
                    return new GameCoreToolkit(root).GameCoreScearh(text);
                }
                catch {}

                return null;
            });

            return cores is null ? new() : cores.ToObservableCollection();
        }

        public static async ValueTask CompLexGameCoreInstallAsync(string version, string name, Action<string, float> action, IEnumerable<ModsPacksModLoaderModel> modloader) {
            InstallerBase<InstallerResponse> installer = null;

            foreach (var mod in modloader) {
                if (mod.Id.Contains("forge")) {
                    var buildResult = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(version)).AsEnumerable();
                    var result = buildResult.Where(x => mod.Id.Contains(x.ForgeVersion))?.FirstOrDefault();

                    installer = new ForgeInstaller(App.LaunchInfoData.GameDirectoryPath, result!, App.LaunchInfoData.JavaRuntimePath?.JavaPath!, name);
                }

                if(mod.Id.Contains("fabric")) {
                    var buildResult = (await FabricInstaller.GetFabricBuildsByVersionAsync(version)).AsEnumerable();
                    var result = buildResult.Where(x => mod.Id.Contains(x.Loader.Version))?.FirstOrDefault();

                    installer = new FabricInstaller(App.LaunchInfoData.GameDirectoryPath, result, name);
                }
            }

            installer!.ProgressChanged += async (_, x) => {
                var progress = x.Progress * 100;
                action($"{Math.Round(progress, 2)}%", progress);
                await Task.Delay(1000);
            };

            var installResult = await Task.Run(async () => await installer.InstallAsync());
            installResult.Success.ShowLog();
        }

        public static async ValueTask CompLexGameCoreInstallAsync(string name, Action<string, float> action, Dependencies dependencies)
        {
            InstallerBase<InstallerResponse> installer = null;

            if (GameCoreToolkit.GetGameCore(App.LaunchInfoData.GameDirectoryPath,name) == null) {
                if (!string.IsNullOrEmpty(dependencies.QuiltLoader)) {               
                    var buildResult = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(dependencies.Minecraft)).AsEnumerable();
                    var result = buildResult.Where(x => dependencies.QuiltLoader.Contains(x.Loader.Version))?.FirstOrDefault();

                    installer = new QuiltInstaller(App.LaunchInfoData.GameDirectoryPath, result, name);
                }
                else {               
                    var buildResult = (await FabricInstaller.GetFabricBuildsByVersionAsync(dependencies.Minecraft)).AsEnumerable();
                    var result = buildResult.Where(x => dependencies.FabricLoader.Contains(x.Loader.Version))?.FirstOrDefault();

                    installer = new FabricInstaller(App.LaunchInfoData.GameDirectoryPath, result, name);
                }
            }
            else return;

            installer!.ProgressChanged += async (_, x) => {
                var progress = x.Progress * 100;
                action($"{Math.Round(progress, 2)}%", progress);
                await Task.Delay(1000);
            };

            var installResult = await Task.Run(async () => await installer.InstallAsync());
            installResult.Success.ShowLog();
        }

        public static async ValueTask<string> GetTotalSizeAsync(GameCore id) {       
            double total = 0;
            foreach (var library in id.LibraryResources) {           
                if (library.Size != 0)
                    total += library.Size;
                else if (library.Size == 0 && library.ToFileInfo().Exists)
                    total += library.ToFileInfo().Length;
            }

            try {           
                var assets = await new ResourceInstaller(id).GetAssetResourcesAsync();

                foreach (var asset in assets) {
               
                    if (asset.Size != 0)
                        total += asset.Size;
                    else if (asset.Size == 0 && asset.ToFileInfo().Exists)
                        total += asset.ToFileInfo().Length;
                }
            }
            catch { }

            return $"{double.Parse(((double)total / (1024 * 1024)).ToString("0.00"))} MB";
        }
    }
}
