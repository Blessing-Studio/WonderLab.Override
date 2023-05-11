using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System.Threading.Tasks;
using wonderlab.control.Animation;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class GameCoreConfigPage : UserControl {   

        public static GameCoreConfigPageViewModel ViewModel { get; set; }
        public GameCoreConfigPage() {       
            InitializeComponent();
            DataContext = ViewModel;

            foreach (ToggleButton item in ButtonGroup.Children) {
                item!.Click += (x, e) => {
                    foreach (ToggleButton item1 in ButtonGroup.Children) {                   
                        item1.IsChecked = false;
                    }

                    item.IsChecked = true;
                };
            }
        }

        public GameCoreConfigPage(GameCore core) { 
            InitializeComponent();
            DataContext = ViewModel = new(core);
            Initialize();

            foreach (ToggleButton item in ButtonGroup.Children) {           
                item!.Click += (x, e) => {
                    foreach (ToggleButton item1 in ButtonGroup.Children) {                   
                        item1.IsChecked = false;
                    }

                    item.IsChecked = true;
                };
            }
        }

        public async void Initialize() {
            await Task.Delay(300);
            TopBar.Margin = new(0);
        }
    }
}
