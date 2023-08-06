using Avalonia.Controls;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
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

                if (GlobalResources.LauncherData.CurrentDownloadAPI is DownloadApiType.Mcbbs) {
                    mcbbs.IsChecked = true;
                } else if (GlobalResources.LauncherData.CurrentDownloadAPI is DownloadApiType.Bmcl) {
                    bmcl.IsChecked = true;
                } else if (GlobalResources.LauncherData.CurrentDownloadAPI is DownloadApiType.Mojang) {
                    official.IsChecked = true;
                }
            } else {
                mcbbs.IsChecked = true;
            }
        }
    }
}
