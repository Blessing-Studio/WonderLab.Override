using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Launch;
using System.Collections.Generic;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs {
    public partial class CrashDialogContent : UserControl {
        public CrashDialogContent() {
            InitializeComponent();
            DataContext = new CrashDialogContentViewModel();
        }

        public CrashDialogContent(MinecraftLaunchResponse response, List<string> logs) {
            InitializeComponent();
            DataContext = new CrashDialogContentViewModel(response, logs);
        }
    }
}
