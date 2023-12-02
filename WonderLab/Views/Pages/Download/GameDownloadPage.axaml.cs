using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.Download;

namespace WonderLab.Views.Pages.Download {
    public partial class GameDownloadPage : ReactiveUserControl<GameDownloadPageViewModel> {
        public GameDownloadPage(GameDownloadPageViewModel vm) {
            InitializeComponent();
            ViewModel = vm;
        }

        public GameDownloadPage() {
            InitializeComponent();
        }
    }
}
