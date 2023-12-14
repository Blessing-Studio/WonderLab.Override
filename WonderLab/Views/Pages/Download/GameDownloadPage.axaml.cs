using Avalonia.Controls;
using WonderLab.ViewModels.Pages.Download;

namespace WonderLab.Views.Pages.Download {
    public partial class GameDownloadPage : UserControl {
        public GameDownloadPageViewModel ViewModel { get; set; }

        public GameDownloadPage(GameDownloadPageViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }

        public GameDownloadPage() {
            InitializeComponent();
        }
    }
}
