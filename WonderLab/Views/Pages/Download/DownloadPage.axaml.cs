using Avalonia.ReactiveUI;
using WonderLab.ViewModels.Pages.Download;

namespace WonderLab.Views.Pages.Download {
    public partial class DownloadPage : ReactiveUserControl<DownloadPageViewModel> {
        public DownloadPage(DownloadPageViewModel vm) {
            InitializeComponent();
            ViewModel = vm;
        }
    }
}
