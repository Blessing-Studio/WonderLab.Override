using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Net.Http.Json;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class NetConfigPage : UserControl {   
        public static NetConfigPageViewModel ViewModel { get; set; } = new();
        public NetConfigPage() {       
            InitializeComponent();
            DataContext = ViewModel;

            if (!GlobalResources.LauncherData.IsNull() && !GlobalResources.LauncherData.IssuingBranch.IsNull()) {
                if (GlobalResources.LauncherData.IssuingBranch == IssuingBranch.Lsaac) {
                    lsaac.IsChecked = true;
                } else {
                    albert.IsChecked = true;
                }

                if (GlobalResources.LauncherData.CurrentDownloadAPI.Host == APIManager.Mcbbs.Host) {
                    mcbbs.IsChecked = true;
                } else if (GlobalResources.LauncherData.CurrentDownloadAPI.Host == APIManager.Bmcl.Host) {
                    bmcl.IsChecked = true;
                } else if (GlobalResources.LauncherData.CurrentDownloadAPI.Host == APIManager.Mojang.Host) {
                    official.IsChecked = true;
                }
            } else {
                mcbbs.IsChecked = true;
            }
        }
    }
}
