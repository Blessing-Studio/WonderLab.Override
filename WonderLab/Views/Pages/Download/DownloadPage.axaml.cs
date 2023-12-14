using Avalonia.Controls;
using WonderLab.ViewModels.Pages.Download;

namespace WonderLab.Views.Pages.Download {
    public partial class DownloadPage : UserControl {
        public DownloadPageViewModel ViewModel { get; set; }

        public DownloadPage(DownloadPageViewModel vm) {
            InitializeComponent();
            DataContext = ViewModel = vm;
        }
    }
}
