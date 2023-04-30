using MinecaftOAuth.Authenticator;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages
{
    public class HomePageViewModel : ReactiveObject
    {
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
        public GameCore SelectGameCore { get; set; }
        
        [Reactive]
        public ObservableCollection<GameCore> GameCores { get; set; } = new();

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(SearchCondition)) {
                SeachGameCore(SearchCondition);
            }

            if (e.PropertyName is nameof(SelectGameCore) && SelectGameCore != null) {
                App.LaunchInfoData.SelectGameCore = SelectGameCore.Id!;
                SelectGameCoreId = SelectGameCore.Id!;
            }
        }

        public async void SeachGameCore(string text) {
            if (!GameCores.Any()) {             
                return;
            }

            GameCores.Clear();
            GameCores = (await GameCoreUtils.SearchGameCoreAsync(App.LaunchInfoData.GameDirectoryPath, text))
                .Distinct().ToObservableCollection();

            if (!GameCores.Any()) {
                SearchSuccess = 1;
            }
            else SearchSuccess = 0;            
        }

        public async void GetGameCoresAction() {
            GameCores.Clear();
            var cores = await GameCoreUtils.GetLocalGameCores(App.LaunchInfoData.GameDirectoryPath);
            HasGameCore = cores.Any() ? 0 : 1;

            foreach (var i in cores) {
                await Task.Delay(20);    
                GameCores.Add(i);                
            }
        }

        public async void SelectAccountAction() {
            var user = await GameAccountUtils.GetUsersAsync().ToListAsync();

            if (user.Count() == 1) {
                CurrentAccount = user.First().Data.ToAccount();
                LaunchTaskAction();
                return;
            }

            MainWindow.Instance.Auth.Show();            
        }

        public async void LaunchTaskAction() {
            $"开始尝试启动游戏 \"{SelectGameCoreId}\"，您可以点击此条进入通知中心以查看启动进度！".ShowMessage(() => {
                MainWindow.Instance.NotificationCenter.Open();
            });

            NotificationViewData data = new() { 
                Title = $"游戏 {SelectGameCoreId} 的启动任务"
            };

            data.TimerStart();
            if (!Path.Combine(JsonUtils.DataPath, "authlib-injector.jar").IsFile()) {
                var result = await HttpWrapper.HttpDownloadAsync("https://download.mcbbs.net/mirrors/authlib-injector/artifact/45/authlib-injector-1.1.45.jar",
                    JsonUtils.DataPath, "authlib-injector.jar");

                Trace.WriteLine($"[信息] Http状态码为 {result.HttpStatusCode}");
            }

            //异步刷新游戏账户
            try {           
                await Task.Run(async () => {
                    if(CurrentAccount.Type == AccountType.Yggdrasil) {
                        YggdrasilAuthenticator authenticator = new YggdrasilAuthenticator((CurrentAccount as YggdrasilAccount)!.YggdrasilServerUrl, "", "");
                        var result = await authenticator.RefreshAsync((CurrentAccount as YggdrasilAccount)!);
                
                        CurrentAccount = result;
                        await GameAccountUtils.RefreshUserDataAsync((await GameAccountUtils.GetUsersAsync().ToListAsync()).Where(x => x.Data.Uuid == result.Uuid.ToString()).First().Data, result);
                    }
                    else if (CurrentAccount.Type == AccountType.Microsoft) {
                        MicrosoftAuthenticator authenticator = new(MinecaftOAuth.Module.Enum.AuthType.Refresh) {
                            ClientId = "9fd44410-8ed7-4eb3-a160-9f1cc62c824c",
                            RefreshToken = (CurrentAccount as MicrosoftAccount)!.RefreshToken!
                        };
                
                        var result = await authenticator.AuthAsync(x => Trace.WriteLine($"[信息] 当前验证步骤 {x}"));
                        CurrentAccount = result;
                        await GameAccountUtils.RefreshUserDataAsync((await GameAccountUtils.GetUsersAsync().ToListAsync()).Where(x => x.Data.Uuid == result.Uuid.ToString()).First().Data, result);
                    }
                });
            }
            catch (Exception ex) {
                $"账户刷新失败，详细信息：{ex.Message}".ShowMessage("Error");
            }


            var config = new LaunchConfig()
            {
                JvmConfig = new()
                {
                    AdvancedArguments = new List<string>() { GetJvmArguments() },
                    MaxMemory = App.LaunchInfoData.MaxMemory,
                    MinMemory = App.LaunchInfoData.MiniMemory,
                    JavaPath = GetCurrentJava().ToFile(),
                },
                
                Account = CurrentAccount,
                WorkingFolder = GameCoreToolkit.GetGameCore(App.LaunchInfoData.GameDirectoryPath, SelectGameCoreId).GetGameCorePath().ToDirectory()!,
            };

            JavaMinecraftLauncher launcher = new(config, App.LaunchInfoData.GameDirectoryPath, true);

            NotificationCenterPage.ViewModel.Notifications.Add(data);
            using var gameProcess = await launcher.LaunchTaskAsync(App.LaunchInfoData.SelectGameCore, x => { 
                Trace.WriteLine($"[信息] {x.Item2}");
                data.Progress = $"{x.Item2} - {Math.Round(x.Item1 * 100, 2)}%";
                data.ProgressOfBar = Math.Round(x.Item1 * 100, 2);
            });

            data.TimerStop();

            data.ProgressOfBar = 100;
            if (gameProcess.State is LaunchState.Succeess) {
                data.Progress = $"启动成功 - 100%";
                $"游戏 \"{App.LaunchInfoData.SelectGameCore}\" 已启动成功，总用时 {data.RunTime}".ShowMessage("启动成功");

                gameProcess.Process.Exited += (sender, e) => {
                    Trace.WriteLine("[信息] 游戏退出！");
                };
                gameProcess.ProcessOutput += ProcessOutput;
            }
            else {
                data.Progress = $"启动失败 - 100%";
                $"游戏 \"{App.LaunchInfoData.SelectGameCore}\" 启动失败，详细信息 {gameProcess.Exception.Message}".ShowMessage("我日，炸了");
            }
        }

        public string GetCurrentJava() {
            if (App.LaunchInfoData.IsAutoSelectJava) {
                var first = App.LaunchInfoData.JavaRuntimes.Where(x => x.Is64Bit && 
                x.JavaSlugVersion == new GameCoreToolkit(App.LaunchInfoData.GameDirectoryPath)
                .GetGameCore(App.LaunchInfoData.SelectGameCore).JavaVersion);                

                if (first.Any()) { 
                    return first.First().JavaPath.ToJavaw();   
                } else {
                    var second = App.LaunchInfoData.JavaRuntimes.Where(x => x.JavaSlugVersion == new GameCoreToolkit(App.LaunchInfoData.GameDirectoryPath)
                   .GetGameCore(App.LaunchInfoData.SelectGameCore).JavaVersion);
                    
                    return second.Any() ? second.First().JavaPath.ToJavaw() : App.LaunchInfoData.JavaRuntimePath.JavaPath?.ToJavaw() ?? string.Empty;
                }
            }

            return App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw();
        }

        public string GetJvmArguments() {
            if (CurrentAccount.Type == AccountType.Yggdrasil) { 
                var account = CurrentAccount as YggdrasilAccount;
                return $"-javaagent:{Path.Combine(JsonUtils.DataPath, "authlib-injector.jar")}={account!.YggdrasilServerUrl}";
            }

            return string.Empty;
        }

        private void ProcessOutput(object? sender, IProcessOutput e) {
            Trace.WriteLine(e.Raw);
        }
    }
}
