using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Linq;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages
{
    public partial class SelectConfigPage : UserControl {
        public static SelectConfigPageViewModel ViewModel { get; set; }

        public SelectConfigPage() {       
            InitializeComponent();
            DataContext = ViewModel = new();

            var togglebuttonGroup = ButtonGroup.Children.Where(x => x.Name != "button");
            foreach (var item in togglebuttonGroup) {
                (item as ToggleButton)!.Click += (x, e) => {
                    foreach (var button in togglebuttonGroup) {                   
                        (button as ToggleButton)!.IsChecked = false;
                    }

                    (x as ToggleButton)!.IsChecked = true;
                };
            }

            button.Click += (x, e) => {
                MainWindow.Instance.NavigationPage(new ActionCenterPage());
            };
        }
    }
}
