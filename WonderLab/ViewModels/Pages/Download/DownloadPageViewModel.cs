using ReactiveUI.Fody.Helpers;

namespace WonderLab.ViewModels.Pages.Download {
    public class DownloadPageViewModel : ViewModelBase {
        public DownloadPageViewModel(GameDownloadPageViewModel page) {
            Current = page;
        }

        [Reactive]
        public object Current { get; set; }
    }
}
