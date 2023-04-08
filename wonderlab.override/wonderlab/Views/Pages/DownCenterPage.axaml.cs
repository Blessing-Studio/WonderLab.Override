using Avalonia.Controls;
using System.Threading.Tasks;
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
        }

        private async void Loaded(object? sender, System.EventArgs e) {       
            await Task.Delay(100);
            TopBar.Margin = new(0);
            BottomBar.Spacing = 15;
        }
    }
}
