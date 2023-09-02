using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utils;
using MinecraftOAuth.Authenticator;
using MinecraftOAuth.Module.Enum;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control;
using wonderlab.control.Animation;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class HomePageViewModel : ViewModelBase {
        public HomePageViewModel() {
            this.PropertyChanged += OnPropertyChanged;
        }

        public bool Isopen { get; set; } = false;

        public Account CurrentAccount { get; set; } = Account.Default;

        [Reactive]
        public string SelectGameCoreId { get; set; }

        [Reactive]
        public string SearchCondition { get; set; }

        [Reactive]
        public double SearchSuccess { get; set; } = 0;

        [Reactive]
        public double HasGameCore { get; set; } = 0;

        [Reactive]
        public double PanelHeight { get; set; } = 0;

        [Reactive]
        public GameCoreViewData SelectGameCore { get; set; }

        [Reactive]
        public ObservableCollection<GameCoreViewData> GameCores { get; set; } = new();

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(SearchCondition)) {
                SeachGameCore(SearchCondition);
            }

            if (e.PropertyName is nameof(SelectGameCore) && SelectGameCore != null) {
                GlobalResources.LaunchInfoData.SelectGameCore = SelectGameCore.Data.Id!;
                SelectGameCoreId = SelectGameCore.Data.Id!;
            }
        }

        public async void SeachGameCore(string text) {
            if (!GameCores.Any()) {
                return;
            }

            GameCores.Clear();
            var result = (await GameCoreUtils.SearchGameCoreAsync(GlobalResources.LaunchInfoData.GameDirectoryPath, text)).Distinct();
            GameCores.Load(result.Select(x => x.CreateViewData<GameCore, GameCoreViewData>()));

            if (!GameCores.Any()) {
                SearchSuccess = 1;
            } else SearchSuccess = 0;
        }

        public async void GetGameCoresAction() {
            GameCores.Clear();
            if (string.IsNullOrEmpty(GlobalResources.LaunchInfoData.GameDirectoryPath)) {
                HasGameCore = 1;
                if(GlobalResources.LaunchInfoData.GameDirectorys.Any())
                {
                    "GameDirectoryError1".GetText().ShowMessage("提示", async () =>
                    {
                        OpenLaunchConfigAction();
                    });
                }
                else
                {
                    "GameDirectoryError2".GetText().ShowMessage("提示", async () =>
                    {
                        OpenLaunchConfigAction();
                    });
                }
                return;
            }

            var cores = await GameCoreUtils.GetLocalGameCores(GlobalResources.LaunchInfoData.GameDirectoryPath);
            HasGameCore = cores.Any() ? 0 : 1;
            GameCores.Load(cores.Select(x => x.CreateViewData<GameCore, GameCoreViewData>()));
        }

        public void SelectAccountAction() {
            try {
                var user = CacheResources.Accounts;
                DialogPage.ViewModel.GameAccounts = user;

                if (user.Count > 1) {
                    App.CurrentWindow.DialogHost.AccountDialog.ShowDialog();
                    return;
                } else if (user.Count <= 0) {
                    "未添加任何账户，无法继续启动步骤，您可以点击此条以转到账户中心！".ShowMessage("提示", async () => {
                        OpenActionCenterAction();
                        await Task.Delay(1000);
                        new AccountPage().Navigation();
                    });
                    return;
                }

                CurrentAccount = user.First().Data.ToAccount();
                LaunchTaskAction();
            }
            catch (Exception) {

            }
            finally {
                GC.Collect();
            }
        }

        public async void LaunchTaskAction() {
            int modCount = 0;
            bool canLaunch = true;
            JavaInfo javaInfo = null!;
            LaunchConfig config = null!;
            NotificationViewData data = null!;

            if (NotificationCenterPage.ViewModel.Notifications.Where(x => {
                if (x.NotificationType is NotificationType.Install && x.Title.Contains(SelectGameCoreId)) {
                    return true;
                }

                return false;
            }).Count() > 0) {
                $"检测到游戏核心 \"{SelectGameCoreId}\" 仍有安装任务正在进行，无法进行启动步骤！".ShowMessage();
                return;
            }

            await PreUIProcessingAsync();

            var gameCore = GameCoreUtil.GetGameCore(GlobalResources.LaunchInfoData.GameDirectoryPath, SelectGameCoreId);
            await DownloadAuthlibAsync();

            //Modpack 重复处理
            await ModpackCheckAsync();

            //异步刷新游戏账户
            await AccountRefreshAsync();

            //游戏依赖检查
            await ResourcesCheckOutAsync();

            await PreConfigProcessingAsync();

            JavaMinecraftLauncher launcher = new(config, GlobalResources.LaunchInfoData.GameDirectoryPath);
            var gameProcess = await launcher.LaunchTaskAsync(GlobalResources.LaunchInfoData.SelectGameCore, x => {
                x.Item2.ShowLog();
            });

            await PostUIProcessingAsync();

            IEnumerable<string> GetAdvancedArguments() {
                if (SystemUtils.IsMacOS) {
                    yield return $"-Xdock:name=Minecraft {gameCore!.Source ?? gameCore.InheritsFrom}";
                    yield return $"-Xdock:icon={Path.Combine(gameCore.Root.FullName, "assets", "objects", "f0","f00657542252858a721e715a2e888a9226404e35")}";
                }

                if (!string.IsNullOrEmpty(GlobalResources.LaunchInfoData.JvmArgument)) {
                    yield return GlobalResources.LaunchInfoData.JvmArgument;
                }

                var authlibJvm = GetAuthlibJvmArguments();
                if (!string.IsNullOrEmpty(authlibJvm)) {
                    yield return authlibJvm;
                }

                string GetAuthlibJvmArguments() {
                    if (CurrentAccount.Type == AccountType.Yggdrasil) {
                        var account = CurrentAccount as YggdrasilAccount;
                        return $"-javaagent:{Path.Combine(JsonUtils.DataPath, "authlib-injector.jar")}={account!.YggdrasilServerUrl}";
                    }

                    return string.Empty;
                }
            }

            async ValueTask PreConfigProcessingAsync() {
                data.Progress = "开始启动步骤 - 0%";
                bool flag = !GlobalResources.LaunchInfoData.IsAutoSelectJava && GlobalResources.LaunchInfoData.JavaRuntimePath.Equals(null);//手动选择 Java 的情况
                javaInfo = await Task.Run(() => flag ? GlobalResources.LaunchInfoData.JavaRuntimePath! : GetCurrentJava());//当选择手动时没有任何问题就手动选择，其他情况一律使用自动选择
                config = new LaunchConfig() {
                    JvmConfig = new() {
                        MaxMemory = GlobalResources.LaunchInfoData.IsAutoGetMemory
                        ? GameCoreUtils.GetOptimumMemory(!gameCore.HasModLoader, modCount).ToInt32()
                        : GlobalResources.LaunchInfoData.MaxMemory,
                        JavaPath = SystemUtils.IsWindows ? javaInfo!.JavaPath.ToJavaw().ToFile() : javaInfo!.JavaPath.ToFile(),
                        AdvancedArguments = GetAdvancedArguments(),
                    },
                    GameWindowConfig = new() {
                        Width = GlobalResources.LaunchInfoData.WindowWidth,
                        Height = GlobalResources.LaunchInfoData.WindowHeight,
                        IsFullscreen = GlobalResources.LaunchInfoData.WindowHeight == 0 && GlobalResources.LaunchInfoData.WindowWidth == 0,
                    },
                    Account = CurrentAccount,
                    IsEnableIndependencyCore = true
                };
            }

            async ValueTask PreUIProcessingAsync() {
                await Dispatcher.UIThread.InvokeAsync(() => {
                    $"开始尝试启动游戏 \"{SelectGameCoreId}\"，您可以点击此条进入通知中心以查看启动进度！".ShowMessage(App.CurrentWindow.NotificationCenter.Open);
                    data = new(NotificationType.Launch) {
                        Title = $"游戏 {SelectGameCoreId} 的启动任务"
                    };

                    data.TimerStart();
                    NotificationCenterPage.ViewModel.Notifications.Add(data);
                });

                await Task.Delay(1500);
            }

            async ValueTask PostUIProcessingAsync() {
                data.ProgressOfBar = 100;
                if (gameProcess.State is LaunchState.Succeess) {
                    data.Progress = $"启动成功 - 100%";
                    $"游戏 \"{GlobalResources.LaunchInfoData.SelectGameCore}\" 已启动成功，总用时 {data.RunTime}".ShowMessage("启动成功");
                    var viewData = await Task.Run(() => gameProcess.CreateViewData<MinecraftLaunchResponse, MinecraftProcessViewData>(CurrentAccount, javaInfo));

                    Dispatcher.UIThread.Post(() => {
                        CacheResources.GameProcesses.Add(viewData);
                        gameProcess.ProcessOutput += ProcessOutput;
                        gameProcess.Process.Exited += (sender, e) => {
                            "Game exit".ShowLog();
                            CacheResources.GameProcesses.Remove(viewData);
                        };
                    }, DispatcherPriority.Background);
                } else {
                    await Dispatcher.UIThread.InvokeAsync(() => {
                        data.Progress = $"启动失败 - 100%";
                        $"游戏 \"{GlobalResources.LaunchInfoData.SelectGameCore}\" 启动失败，详细信息 {gameProcess.Exception}".ShowInfoDialog("程序遭遇了异常");
                    });
                }

                data.TimerStop();
            }

            async ValueTask DownloadAuthlibAsync() {
                if (!Path.Combine(JsonUtils.DataPath, "authlib-injector.jar").IsFile() && CurrentAccount.Type is AccountType.Yggdrasil) {
                    data!.Progress = "下载 Authlib-Injector 中";

                    var result = await Task.Run(async () => {
                        return await HttpUtil.HttpDownloadAsync("https://download.mcbbs.net/mirrors/authlib-injector/artifact/45/authlib-injector-1.1.45.jar",
                            JsonUtils.DataPath, "authlib-injector.jar");
                    });
                }
            }

            async ValueTask ModpackCheckAsync() {
                if (gameCore.GetModsPath().IsDirectory()) {
                    data!.Progress = "开始检查 Mod";

                    try {
                        var result = await Task.Run(async () => {
                            ModPackUtil toolkit = new(gameCore, true);
                            var modpacks = (await toolkit.LoadAllAsync()).Where(x => x.IsEnabled);
                            modCount = modpacks.Count();
                            return modpacks.GroupBy(i => i.Id).Where(g => g.Count() > 1);
                        });

                        if (result.Count() > 0) {
                            foreach (var item in result) {
                                $"模组 \"{item.ToList().First().FileName}\" 在此文件夹已有另一版本，可能导致游戏无法正常启动，已中止启动操作！".ShowMessage();
                                return;
                            }
                        }
                    }
                    catch (Exception) {

                    }
                }
            }

            async ValueTask AccountRefreshAsync() {
                try {
                    data.Progress = "开始刷新账户";

                    await Task.Run(async () => {
                        if (CurrentAccount.Type == AccountType.Yggdrasil) {
                            var userData = CacheResources.Accounts.Where(x => x.Data.Uuid == CurrentAccount.Uuid.ToString()).First().Data;
                            YggdrasilAuthenticator authenticator = new YggdrasilAuthenticator((CurrentAccount as YggdrasilAccount)!.YggdrasilServerUrl,
                                userData.Email, userData.Password);
                            var result = await authenticator.RefreshAsync((CurrentAccount as YggdrasilAccount)!);

                            if (result.IsNull()) {
                                "需重新验证".ShowLog();
                            }

                            CurrentAccount = result;
                            await AccountUtils.RefreshAsync(CacheResources.Accounts.Where(x => x.Data.Uuid == result.Uuid.ToString()).First().Data, result);
                        } else if (CurrentAccount.Type == AccountType.Microsoft) {
                            MicrosoftAuthenticator authenticator = new(AuthType.Refresh) {
                                ClientId = GlobalResources.ClientId,
                                RefreshToken = (CurrentAccount as MicrosoftAccount)!.RefreshToken!
                            };

                            var result = await authenticator.AuthAsync(x => data.Progress = $"当前步骤：{x}");
                            CurrentAccount = result;
                            await AccountUtils.RefreshAsync(CacheResources.Accounts.Where(x => x.Data.Uuid == result.Uuid.ToString()).First().Data, result);
                        }
                    });
                }
                catch (Exception ex) {
                    $"账户刷新失败，详细信息：{ex}".ShowInfoDialog("程序遭遇了异常");
                    data.TimerStop();
                    return;
                }
            }

            async ValueTask ResourcesCheckOutAsync() {
                try {
                    await Task.Run(async () => {
                        ResourceInstaller installer = new(new GameCoreUtil(GlobalResources.LaunchInfoData.GameDirectoryPath)
                            .GetGameCore(GlobalResources.LaunchInfoData.SelectGameCore));

                        data.Progress = $"开始检查并补全丢失的资源";
                        var result = await installer.DownloadAsync((s, f) => {
                            try {
                                Dispatcher.UIThread.Post(() => {
                                    var value = Math.Round(f * 100, 2);
                                    data.ProgressOfBar = value;
                                }, DispatcherPriority.Background);
                            }
                            catch (Exception) {
                            }
                        });
                    });
                }
                catch (Exception) {
                }
            }
        }

        public async void ImportModpacksAction() {
            var result = await DialogUtils.OpenFilePickerAsync(new List<FilePickerFileType>() {
                new("整合包文件") { Patterns = new List<string>() { "*.zip", "*.mrpack" } }
            }, "请选择整合包文件");

            if (result!.IsNull()) {
                return;
            }
            var type = ModpacksUtils.ModpacksTypeAnalysis(result.FullName);
            if (type is ModpacksType.Unknown) {
                "未知整合包类型".ShowMessage();
                return;
            }

            await ModpacksUtils.ModpacksInstallAsync(result.FullName, type);
        }

        public void OpenActionCenterAction() {
            var back = App.CurrentWindow.Back;
            OpacityChangeAnimation opacity = new(false) {
                RunValue = 0,
            };
            
            opacity.RunAnimation(back);
            App.CurrentWindow.CloseTopBar();
            new ActionCenterPage().Navigation();
        }

        public void OpenConsoleAction() {
            var back = App.CurrentWindow.Back;
            OpacityChangeAnimation opacity = new(false) {
                RunValue = 0,
            };

            opacity.RunAnimation(back);
            App.CurrentWindow.CloseTopBar();
            new ConsoleCenterPage().Navigation();
        }

        public void OpenLaunchConfigAction()
        {
            var back = App.CurrentWindow.Back;
            OpacityChangeAnimation opacity = new(false)
            {
                RunValue = 0,
            };

            opacity.RunAnimation(back);
            App.CurrentWindow.CloseTopBar();
            new LaunchConfigPage().Navigation();
        }

        public JavaInfo GetCurrentJava() {
            var first = GlobalResources.LaunchInfoData.JavaRuntimes.Where(x => x.Is64Bit &&
            x.JavaSlugVersion == new GameCoreUtil(GlobalResources.LaunchInfoData.GameDirectoryPath)
            .GetGameCore(GlobalResources.LaunchInfoData.SelectGameCore).JavaVersion);

            if (first.Any()) {
                return first.First();
            } else {
                var second = GlobalResources.LaunchInfoData.JavaRuntimes.Where(x => x.JavaSlugVersion == new GameCoreUtil(GlobalResources.LaunchInfoData.GameDirectoryPath)
               .GetGameCore(GlobalResources.LaunchInfoData.SelectGameCore).JavaVersion);

                return second.Any() ? second.First() : GlobalResources.LaunchInfoData.JavaRuntimePath;
            }
        }

        private void ProcessOutput(object? sender, IProcessOutput e) {
            Trace.WriteLine(e.Raw);
        }
    }
}

//TODO: Fuck MacOS
#region Launch Script Create Method
//string java = (SystemUtils.IsWindows ? Path.Combine(new FileInfo(javaInfo!.JavaPath).Directory.FullName,"java.exe") : javaInfo!.JavaPath.ToFile().FullName);
//StringBuilder builder = new();
//if (SystemUtils.IsWindows) {
//    builder.AppendLine("@echo off");
//    builder.AppendLine($"set APPDATA={gameCore.Root.Root.FullName}");
//    builder.AppendLine($"set INST_NAME={gameCore.Id}");
//    builder.AppendLine($"set INST_ID={gameCore.Id}");
//    builder.AppendLine($"set INST_DIR={gameCore.GetGameCorePath()}");
//    builder.AppendLine($"set INST_MC_DIR={gameCore.Root.FullName}");
//    builder.AppendLine($"set INST_JAVA=\"{java}\"");
//    builder.AppendLine($"cd /D {gameCore.Root.FullName}");
//    builder.AppendLine($"\"{java}\" {string.Join(' '.ToString(), gameProcess.Arguemnts)}");
//    builder.AppendLine($"pause");
//} else if (SystemUtils.IsMacOS) {
//    builder.AppendLine($"export INST_NAME={gameCore.Id}");
//    builder.AppendLine($"export INST_ID={gameCore.Id}");
//    builder.AppendLine($"export INST_DIR=\"{gameCore.GetGameCorePath(launcher.EnableIndependencyCore)}\"");
//    builder.AppendLine($"export INST_MC_DIR=\"{gameCore.Root.FullName}\"");
//    builder.AppendLine($"export INST_JAVA=\"{java}\"");
//    builder.AppendLine($"cd \"{gameCore.Root!.FullName}\"");
//    builder.AppendLine($"\"{java}\" {string.Join(' '.ToString(), launcher.ArgumentsBuilder.Build())}");
//} else if (SystemUtils.IsLinux) {
//    builder.AppendLine($"export INST_JAVA={java}");
//    builder.AppendLine($"export INST_MC_DIR={gameCore.Root!.FullName}");
//    builder.AppendLine($"export INST_NAME={gameCore.Id}");
//    builder.AppendLine($"export INST_ID={gameCore.Id}");
//    builder.AppendLine($"export INST_DIR={gameCore.GetGameCorePath(launcher.EnableIndependencyCore)}");
//    builder.AppendLine($"cd {gameCore.Root!.FullName}");
//    builder.AppendLine($"{java} {string.Join(' '.ToString(), launcher.ArgumentsBuilder.Build())}");
//}

//File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "launchScript.bat"), builder.ToString());
#endregion
