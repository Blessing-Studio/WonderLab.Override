using Avalonia.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Avalonia.Media;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.control.Animation;
using wonderlab.ViewModels.Pages;
using MinecraftLaunch.Modules.Models.Launch;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Input;
using wonderlab.Views.Windows;
using wonderlab.Class.AppData;
using MinecraftLaunch.Modules.Toolkits;
using wonderlab.Class.ViewData;

namespace wonderlab.Views.Pages
{
    public partial class HomePage : UserControl
    {
        public static HomePageViewModel ViewModel { get; set; }
        public HomePage()
        {
            InitializeComponent();
            DataContext = ViewModel = new();

            Close.Click += CloseClick;
            LaunchButton.Click += OnLaunchButtonClick;
            SelectGameCoreButton.Click += OnGameChangeClick;

            Polymerize.Opacity = 0;
            Dispatcher.UIThread.Post(async () => {
                await Task.Delay(200);
                ViewModel.SelectGameCoreId = GlobalResources.LaunchInfoData.SelectGameCore;
            });

        }

        private void GoConfigClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Button? button = sender as Button;
            App.CurrentWindow.CloseTopBar();
            new GameCoreConfigPage((button!.DataContext! as GameCoreViewData)!)!.Navigation();
        }

        private async void CloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            ViewModel.Isopen = false;
            Polymerize.Opacity = 0;
            await Task.Delay(60);

            ViewModel.PanelHeight = 0;
            Spotlight.IsHitTestVisible = false;
        }
        
        private async void OnGameChangeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            ViewModel.Isopen = true;
            ViewModel.GetGameCoresAction();

            Spotlight.IsHitTestVisible = true;
            ViewModel.PanelHeight = App.CurrentWindow.Height - 180;

            await Task.Delay(60);
            Polymerize.Opacity = 1;
        }

        private void OnLaunchButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            try {
                "无法继续启动步骤，原因：未选择游戏核心".ShowMessage("提示");

                if (string.IsNullOrEmpty(ViewModel.SelectGameCoreId)) {
                    "无法继续启动步骤，原因：未选择游戏核心".ShowMessage("提示");
                    return;
                }

                if (GlobalResources.LaunchInfoData.IsAutoSelectJava && !GlobalResources.LaunchInfoData.JavaRuntimes.HasValue()) {
                    "无法继续启动步骤，原因：未选择 Java 运行时".ShowMessage("提示");
                    return;
                } else if (!GlobalResources.LaunchInfoData.IsAutoSelectJava && !GlobalResources.LaunchInfoData.JavaRuntimes.HasValue()) {
                    "无法继续启动步骤，原因：未选择 Java 运行时".ShowMessage("提示");
                    return;
                }

                //if (GlobalResources.LaunchInfoData.JavaRuntimePath.IsNull() ||
                //    !GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath.IsFile() ||
                //    !GlobalResources.LaunchInfoData.JavaRuntimes.HasValue()) {
                //    "LaunchCheckError1".GetText().ShowMessage("Tip".GetText());
                //    return;
                //}

                ViewModel.SelectAccountAction();
            }
            catch (System.Exception) {
                
            }
        }
    }
}
//Process.Start(new ProcessStartInfo("https://vdse.bdstatic.com//192d9a98d782d9c74c96f09db9378d93.mp4")
//{
//    UseShellExecute = true,
//    Verb = "open"
//});