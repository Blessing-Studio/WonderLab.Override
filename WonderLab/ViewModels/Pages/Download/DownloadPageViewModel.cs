using ReactiveUI.Fody.Helpers;
using WonderLab.Views.Pages.Download;

namespace WonderLab.ViewModels.Pages.Download {
    public class DownloadPageViewModel : ViewModelBase {
        public DownloadPageViewModel(GameDownloadPage page) {
            Current = page;
        }

        [Reactive]
        public object Current { get; set; }
    }
}
