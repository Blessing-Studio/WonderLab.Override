using JetBrains.Annotations;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;

namespace wonderlab.ViewModels.Dialogs
{
    public class GameCrashInfoDialogViewModel : ReactiveObject {
        [Reactive]
        public string CrashInfo { get; set; }

        [Reactive]
        public GameCore GameCore { get; set; }

        [Reactive]
        public Account Account { get; set; } = Account.Default;

        [Reactive]
        public int Memory { get; set; } = App.LaunchInfoData.MaxMemory;

        [Reactive]
        public int JavaVersion { get; set; }

        [Reactive]
        public string OsPlatform { get; set; } = SystemUtils.GetPlatformName();

        [Reactive]
        public ObservableCollection<string> CrashModpacks { get; set; }

        public void HideDialogAction() {
            MainWindow.Instance.GameCrashInfo.CrashDialog.HideDialog();
        }
    }
}
