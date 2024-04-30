using WonderLab.Services;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class NetworkSettingPageViewModel : ViewModelBase {
    private readonly SettingService _settingService;
    private readonly DownloadService _downloadService;
    
    public NetworkSettingPageViewModel(SettingService settingService, DownloadService downloadService) { 
        _settingService = settingService;
        _downloadService = downloadService;
    }
}