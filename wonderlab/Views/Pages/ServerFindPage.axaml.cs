using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class ServerFindPage : UserControl {
        public static ServerFindPageViewModel ViewModel { get; set; } = new();

        public ServerFindPage() {
            Initialized += InitializedAction;
            InitializeComponent();
            DataContext = ViewModel;
        }

        private async void InitializedAction(object? sender, System.EventArgs e) {
            await Task.Delay(100);
            await ViewModel.GetServerListAsync();
        }
    }
}
