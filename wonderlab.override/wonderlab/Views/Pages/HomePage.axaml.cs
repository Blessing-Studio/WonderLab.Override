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

namespace wonderlab.Views.Pages
{
    public partial class HomePage : UserControl
    {
        public bool Isopen { get; set; } = false;
        public static HomePageViewModel ViewModel { get; set; }
        public HomePage()
        {
            InitializeComponent();
            DataContext = ViewModel = new();

            Close.Click += CloseClick;
            bab.LaunchButtonClick += OnLaunchButtonClick;
            bab.GameChangeClick += OnGameChangeClick;

            Polymerize.Opacity = 0;
            Spotlight.Height = 0;
        }

        private async void CloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Isopen = false;
            Polymerize.Opacity = 0;
            await Task.Delay(60);

            Spotlight.Height = 0;
            Spotlight.IsHitTestVisible = false;
        }

        private async void OnGameChangeClick(object? sender, System.EventArgs e) {
            Isopen = true;
            ViewModel.GetGameCoresAction();

            Spotlight.IsHitTestVisible = true;            
            Spotlight.Height = MainWindow.Instance.Height - 160;

            await Task.Delay(60);
            Polymerize.Opacity = 1;
        }

        private void OnLaunchButtonClick(object? sender, System.EventArgs e) {       
            ViewModel.SelectAccountAction();
        }
    }
}
//Process.Start(new ProcessStartInfo("https://vdse.bdstatic.com//192d9a98d782d9c74c96f09db9378d93.mp4")
//{
//    UseShellExecute = true,
//    Verb = "open"
//});