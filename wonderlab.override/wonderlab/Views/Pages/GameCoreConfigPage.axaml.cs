using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using MinecraftLaunch.Modules.Models.Launch;
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

            foreach (ToggleButton item in ButtonGroup.Children) {           
                item!.Click += (x, e) => {
                    foreach (ToggleButton item1 in ButtonGroup.Children) {                   
                        item1.IsChecked = false;
                    }

                    item.IsChecked = true;
                };
            }
        }
    }
}
