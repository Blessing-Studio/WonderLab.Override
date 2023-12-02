using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Utilities;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using WonderLab.Classes.Managers;

namespace WonderLab.Classes.Models.Tasks {
    public class LaunchTask : TaskBase {
        private GameCore _gameCore;

        private ConfigDataModel _config;

        private NotificationManager _notificationManager;

        public LaunchTask(GameCore core, ConfigDataModel config, NotificationManager notificationManager) {
            _gameCore = core;
            _config = config;
            _notificationManager = notificationManager;
            JobName = $"游戏 {core.Id} 的启动任务";
        }

        public async override ValueTask BuildWorkItemAsync(CancellationToken token) {
            CanBeCancelled = false;

            try {
                JavaMinecraftLauncher launcher = new(BuildLaunchConfig(),
                    _config.GameFolder);

                var result = await launcher.LaunchTaskAsync(_gameCore.Id!, args => {
                    ReportProgress(args.Item1 * 100, args.Item2);
                });

                if (result.State == LaunchState.Succeess) {
                    IsIndeterminate = true;
                    await Task.Run(result.Process.WaitForInputIdle);
                    _notificationManager.Info($"游戏 [{_gameCore.Id}] 启动成功");
                } else {
                    Debug.WriteLine($"游戏 [{_gameCore.Id}] 启动失败，异常为[{result.Exception}]");
                }
            }
            catch (Exception) {

            }
        }

        public LaunchConfig BuildLaunchConfig() {
            var javaPath = _config.IsAutoSelectJava 
                ? JavaUtil.GetCorrectOfGameJava(_config.JavaPaths, _gameCore)
                : _config.JavaPath;

            var maxMemory = _config.IsAutoMemory
                ? GetOptimumMemory(!_gameCore.HasModLoader)
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
                Account = Account.Default,
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
