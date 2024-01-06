using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Views.Pages.Download;

namespace WonderLab.ViewModels.Pages.Download;

public partial class DownloadPageViewModel : ViewModelBase {
    public DownloadPageViewModel(GameDownloadPage page) {
        Current = page;
    }

    [ObservableProperty]
    private object current;
}