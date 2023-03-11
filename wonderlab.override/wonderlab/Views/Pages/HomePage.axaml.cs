using Avalonia.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Avalonia.Media;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.control.Animation;
using wonderlab.ViewModels.Pages;
using MinecraftLaunch.Modules.Models.Launch;

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
            bab.LaunchButtonClick += OnLaunchButtonClick;
            bab.GameChangeClick += OnGameChangeClick;

            TransformYAnimation animation = new(true);
            animation.RunTime = 0.0000001;
            animation.RunAnimation(Spotlight);
        }

        private void CloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            TransformYAnimation animation = new(true);
            animation.RunTime = 0.85;
            animation.RunAnimation(Spotlight);

        }

        private void OnGameChangeClick(object? sender, System.EventArgs e) {
            var button = sender as Button;
            var form = Spotlight.RenderTransform as ScaleTransform;
            
            if (form is not null && form.ScaleY is 1) {
                return;
            }

            button.IsEnabled = false;
            ViewModel.GetGameCoresAction();

            TransformYAnimation animation = new(false);
            animation.RunTime = 0.85;
            animation.AnimationCompleted += (_, _) => button.IsEnabled = true;
            animation.RunAnimation(Spotlight);
        }

        private void OnLaunchButtonClick(object? sender, System.EventArgs e) {       
            MainWindow.Instance.ShowInfoBar("Info", $"开始尝试启动游戏 \"{bab.GameCoreId}\"");
            //Process.Start(new ProcessStartInfo("https://vdse.bdstatic.com//192d9a98d782d9c74c96f09db9378d93.mp4")
            //{
            //    UseShellExecute = true,
            //    Verb = "open"
            //});

            ViewModel.LaunchTaskAction();
        }
    }
}
