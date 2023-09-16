using Avalonia.Controls.Documents;
using Avalonia.Threading;
using DialogHostAvalonia;
using DynamicData;
using MinecraftLaunch.Events;
using MinecraftLaunch.Modules.Analyzers;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
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
                CacheOutputs.Add(x.Raw);

                Dispatcher.UIThread.Post(() => {
                    Outputs.Add(InlineUtils.CraftGameLogsInline(result));
                });
            };
        }

        private bool IsLauncherStop = false;

        private void OnExited(object? sender, ExitedArgs e) {       
            if (e.Crashed && !IsLauncherStop) {
                Dispatcher.UIThread.Post(async () => {
                    GameCrashAnalyzer analyzer = new(Outputs.Select(x => x.Text).ToList()!);
                    var analyzerResult = await analyzer.AnalyseAsync();
                    App.CurrentWindow.Activate();

                    await DialogHost.Show(new CrashDialogContent(Data, CacheOutputs),"dialogHost");
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

        [Reactive]
        public ObservableCollection<InlineCollection> Outputs { get; set; } = new();

        /// <summary>
        /// 日志缓存集合
        /// </summary>
        public List<string> CacheOutputs { get; set; } = new();
    }
}
