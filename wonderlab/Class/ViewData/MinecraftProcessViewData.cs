using Avalonia.Controls.Documents;
using Avalonia.Threading;
using DynamicData;
using MinecraftLaunch.Events;
using MinecraftLaunch.Modules.Analyzers;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;
using wonderlab.Views.Dialogs;
using wonderlab.Views.Windows;

namespace wonderlab.Class.ViewData
{
    public class MinecraftProcessViewData : ViewDataBase<MinecraftLaunchResponse> {   
        public MinecraftProcessViewData(MinecraftLaunchResponse data, Account account, JavaInfo info) : base(data) {
            Account = account;
            JavaInfo = info;

            data.Exited += OnExited;
            data.ProcessOutput += (_, x) => {
                RunState = data.Process.Responding ? RunState.Normal : RunState.NotResponding;
                var result = GameLogAnalyzer.AnalyseAsync(x.Raw);

                Dispatcher.UIThread.Post(() => {
                    Outputs.Add(InlineUtils.CraftGameLogsInline(result));
                });
            };
        }

        private bool IsLauncherStop = false;

        private void OnExited(object? sender, ExitedArgs e) {       
            if (e.Crashed && !IsLauncherStop) {
                Dispatcher.UIThread.Post(() => {
                    GameCrashAnalyzer analyzer = new(Outputs.Select(x => x.Text).ToList()!);
                    var analyzerResult = analyzer.AnalyseAsync();
                    var viewModel = GameCrashInfoDialog.ViewModel!;
                    App.CurrentWindow.Activate();

                    viewModel.GameCore = Data.GameCore;
                    viewModel.Account = Account;
                    viewModel.JavaVersion = JavaInfo.JavaSlugVersion;                    

                    App.CurrentWindow.DialogHost.GameCrashInfo.CrashDialog.ShowDialog();
                    if (!analyzerResult.IsNull() && analyzerResult.Count > 0) {
                        viewModel.CrashInfo = string.Join("\n", analyzerResult.Keys.Select(x => x.ToString()));

                        //判断是否具有导致崩溃的模组，没有则直接跳出方法
                        if (analyzerResult.Values.IsNull() || analyzerResult.Count == 0) {
                            return;
                        }

                        foreach (var item in analyzerResult.Values) {                       
                            if(!item.IsNull() && item.Count > 0) {
                                viewModel.CrashModpacks.AddRange(item);
                            }
                        }
                    }
                });
            }
        }

        public void MinecraftStopAction() {   
            if (!Data.Process.HasExited) {
                Data.Stop();
                IsLauncherStop = true;
            }
        }

        [Reactive]
        public JavaInfo JavaInfo { get; private set; }

        [Reactive]
        public Account Account { get; private set; }

        [Reactive]
        public RunState RunState { get; set; }

        /// <summary>
        /// 日志缓存集合
        /// </summary>
        public ObservableCollection<InlineCollection> Outputs { get; set; } = new();
    }
}
