using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WonderLab.Classes.Managers;

namespace WonderLab.Classes.Handlers
{
    public class LaunchHandler {
        private ConfigDataManager _dataManager;

        public LaunchHandler(ConfigDataManager dataManager) {
            _dataManager = dataManager;
        }

        public async ValueTask<MinecraftLaunchResponse> HandleAsync(GameCore core, Account account) {
            LaunchConfig launchConfig = new() {
                Account = account,
                IsChinese = true,
                LauncherName = "WonderLab",
                JvmConfig = new JvmConfig {
                    JavaPath = new(GetJava(core).JavaPath),
                    MaxMemory = _dataManager.Config.MaxMemory,
                    AdvancedArguments = GetAdvancedArguments(account),
                },
                GameWindowConfig = new GameWindowConfig {
                    Width = _dataManager.Config.Width,
                    Height = _dataManager.Config.Height,
                    IsFullscreen = _dataManager.Config.IsFullscreen,
                },
            };

            JavaMinecraftLauncher launcher = new(launchConfig, _dataManager
                .Config.GameFolder);

            return await launcher.LaunchTaskAsync(core.Id!);
        }

        private JavaInfo GetJava(GameCore core) {
            if (!_dataManager.Config.IsAutoSelectJava || _dataManager.Config.JavaPath is null) {
                return null!;
            }

            return JavaUtil.GetCorrectOfGameJava(_dataManager
                .Config.JavaPaths, core);
        }

        private IEnumerable<string> GetAdvancedArguments(Account account) {
            if (account.Type is AccountType.Yggdrasil) {
                yield return "";
            }
        }
    }
}
