using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class ServerFindPage : UserControl {
        public static ServerFindPageViewModel ViewModel { get; set; } = new();

        public ServerFindPage() {
            InitializeComponent();
            DataContext = ViewModel; new Button().Click += CloseClick;
        }

        private void CloseClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            ServerList.SelectedItem = null; 
        }
    }
}
