using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;
using wonderlab.Class.Enum;
using wonderlab.ViewModels.Pages;

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
                };
            }
        }

        private async void Loaded(object? sender, System.EventArgs e) {       
            await Task.Delay(100);
            TopBar.Margin = new(0);
            BottomBar.Spacing = 15;
        }
    }
}
