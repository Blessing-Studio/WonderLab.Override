using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Views.Pages.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class DownloadPageViewModel : ViewModelBase
{
    public DownloadPageViewModel(GameDownloadPage page)
    {
        Current = page;
    }

    [ObservableProperty]
    private object current;
}