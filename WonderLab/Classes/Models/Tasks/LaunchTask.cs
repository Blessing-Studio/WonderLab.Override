using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Components.Resolver;
using MinecraftLaunch.Utilities;
using WonderLab.Services;

namespace WonderLab.Classes.Models.Tasks {
    public class LaunchTask : TaskBase {
        private GameEntry _gameCore;
        private ConfigDataModel _config;
        private DataService _dataManager;
        private NotificationService _notificationManager;

        public LaunchTask(GameEntry core, DataService dataManager, NotificationService notificationManager) {
            _gameCore = core;
            _dataManager = dataManager;
            _config = dataManager.ConfigData;
            _notificationManager = notificationManager;
            JobName = $"游戏 {core.Id} 的启动任务";
        }

        public async override ValueTask BuildWorkItemAsync(CancellationToken token) {
            CanBeCancelled = false;

            try {
                Launcher launcher = new(new GameResolver(_config.GameFolder),BuildLaunchConfig());
                var result = await launcher.LaunchAsync(_gameCore.Id!);

                if (!result.Process.HasExited) {
                    IsIndeterminate = true;
                    await Task.Run(result.Process.WaitForInputIdle);
                    _notificationManager.Info($"游戏 [{_gameCore.Id}] 启动成功");
                } else {
                    Debug.WriteLine($"游戏 [{_gameCore.Id}] 已退出");
                }
            }
            catch (Exception) {

            }
        }

        public LaunchConfig BuildLaunchConfig() {
            var javaPath = _config.IsAutoSelectJava 
                ? JavaUtil.GetCurrentJava(_config.JavaPaths, _gameCore)
                : _config.JavaPath;

            var maxMemory = _config.IsAutoMemory
                ? GetOptimumMemory(!_gameCore.IsVanilla)
                : _config.MaxMemory;

            return new() {
                JvmConfig = new(javaPath.JavaPath) {
                    MaxMemory = Convert.ToInt32(maxMemory),
                },
                GameWindowConfig = new() {
                    Width = _config.Width,
                    Height = _config.Height,
                    IsFullscreen = _config.IsFullscreen,
                },
                Account = new OfflineAuthenticator("Yang114").Authenticate(),
                LauncherName = "WonderLab",
                IsEnableIndependencyCore = _config.IsEnableIndependencyCore,
            };

            double GetOptimumMemory(bool isVanilla, int modCount = 0) {
                //var free = EnvironmentUtil.GetMemoryInfo()
                //    .Free;
                //double cache = 1024;

                //if (isVanilla && modCount == 0) { //原版
                //    cache = (2.5 * 1024) + free / 4;
                //} else if (modCount > 0) {
                //    cache = (3 + modCount / 60) * 1024;
                //    cache = ((free - cache) / 4) + cache;
                //    if (cache > free) {
                //        cache = free - 100;
                //    }
                //} else {
                //    cache = (3 * 1024) + free / 4;
                //}

                //return cache;
                return _config.MaxMemory;
            }
        }
    }
}
