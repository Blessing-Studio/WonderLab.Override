using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using MinecraftLaunch.Modules.Models.Install;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.ViewModels.Pages;
using wonderlab.Views.Dialogs;

namespace wonderlab.Views.Pages
{
    public partial class DownCenterPage : UserControl
    {
        public static DownCenterPageViewModel ViewModel { get; set; } = new();
        public DownCenterPage()
        {
            Initialized += Loaded;
            InitializeComponent();
            DataContext = ViewModel;

            foreach (var item in BottomBar.Children) {           
                (item as ToggleButton)!.Click += (x, e) => {
                    foreach (var button in BottomBar.Children) {                   
                        (button as ToggleButton)!.IsChecked = false;
                    }
                   
                    (x as ToggleButton)!.IsChecked = true;
                    ViewModel.ResourceType = (x as ToggleButton)!.Tag!.ToString() switch {
                        "Minecraft" => ResourceType.Minecraft,
                        "Curseforge" => ResourceType.Curseforge,
                        "Modrinth" => ResourceType.Modrinth,
                        _ => ResourceType.Minecraft,
                    };

                    if(ViewModel.ResourceType == ResourceType.Minecraft) {
                        ViewModel.IsResource = false;
                    }
                    else {     
                        ViewModel.IsResource = true;
                    }
                };
            }
        }

        private async void Loaded(object? sender, System.EventArgs e) {       
            await Task.Delay(100);
            BottomBar.Spacing = 15;
        }

        private void OpenDialogAction(object? sender, RoutedEventArgs args) {
            GameInstallDialog.ViewModel.CurrentGameCore = ((sender as Button)!.DataContext as GameCoreEmtity)!;
            CacheResources.GameCoreInstallInfo = ((sender as Button)!.DataContext as GameCoreEmtity)!;
            ViewModel.OpenGameInstallDialogAction();
        }

        public void GoResourceInfoAction(object? sender, RoutedEventArgs args) {
            var resourceInfo = ((sender as Button)!.DataContext) as WebModpackViewData;
            new WebModpackInfoPage(resourceInfo).Navigation();
        }
    }
}
