using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using WonderLab.Services;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class NetworkSettingPageViewModel : ViewModelBase {
    private readonly SettingService _settingService;
    private readonly DownloadService _downloadService;

    [ObservableProperty] private bool _isUseMirrorDownloadSource;

    [ObservableProperty] private int _multiPartsCount;
    [ObservableProperty] private int _multiThreadsCount;

    public NetworkSettingPageViewModel(SettingService settingService, DownloadService downloadService) {
        _settingService = settingService;
        _downloadService = downloadService;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case "MultiPartsCount":
                _settingService.Data.MultiPartsCount = MultiPartsCount;
                break;
            case "MultiThreadsCount":
                _settingService.Data.MultiThreadsCount = MultiThreadsCount;
                break;
            case "IsUseMirrorDownloadSource":
                _settingService.Data.IsUseMirrorDownloadSource = IsUseMirrorDownloadSource;
                break;
        }

        _downloadService.Init();
    }
}