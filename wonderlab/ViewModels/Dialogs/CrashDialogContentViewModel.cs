using Avalonia.Controls;
using DialogHostAvalonia;
using DynamicData.Kernel;
using MinecraftLaunch.Modules.Analyzers;
using MinecraftLaunch.Modules.Models.Launch;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using wonderlab.control;

namespace wonderlab.ViewModels.Dialogs {
    public class CrashDialogContentViewModel : ViewModelBase {//ActualHeight ActualWidth

        //Debug
        public CrashDialogContentViewModel() {
        }

        public CrashDialogContentViewModel(MinecraftLaunchResponse response, List<string> logs) {
            GameCore = response.GameCore;
            JavaPath = response.Process.StartInfo.FileName;
            MaxMemory = GlobalResources.LaunchInfoData.MaxMemory;

            LogOutputs.Load(logs);
            GameCrashAnalyzer analyzer = new(logs);

            var result = analyzer.AnalyseAsync().Result;
            if (!result.IsNull() && result.Keys != null && result.Keys.Count > 0) {
                CrashInfos.Load(result.Keys.Select(x => x
                    .ToString()));
            }
        }

        [Reactive]
        public bool IsGoLog { get; set; }

        [Reactive]
        public int MaxMemory { get; set; }

        [Reactive]
        public string JavaPath { get; set; }

        [Reactive]
        public GameCore GameCore { get; set; }

        [Reactive]
        public double AnimationWidth { get; set; } = 450;

        [Reactive]
        public double AnimationHeight { get; set; } = 240;

        [Reactive]
        public ObservableCollection<string> CrashInfos { get; set; } = new();

        [Reactive]
        public ObservableCollection<string> LogOutputs { get; set; } = new();

        public string OsPlatform => SystemUtils.GetPlatformName();

        public override void GoBackAction() {
            AnimationWidth = 450;
            AnimationHeight = 240;
            IsGoLog = false;
        }

        public void CloseAction() {
            DialogHost.Close("dialogHost");
        }

        public void GoWacthLogAction() {
            AnimationWidth = 750;
            AnimationHeight = 420;
            IsGoLog = true;
        }
    }
}
