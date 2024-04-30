using MinecraftLaunch.Utilities;
using MinecraftLaunch.Classes.Models.Download;

namespace WonderLab.Services.Download;

/// <summary>
/// 下载服务类
/// </summary>
public sealed class DownloadService {
    private readonly SettingService _settingService;

    public DownloadRequest MainDownloadRequest { get; set; }

    public DownloadService(SettingService settingService) {
        _settingService = settingService;
    }

    public void Init() {
        MainDownloadRequest = new() {
            IsPartialContentSupported = true,
            FileSizeThreshold = 1024 * 1024 * 3,
            MultiPartsCount = _settingService.Data.MultiPartsCount,
            MultiThreadsCount = _settingService.Data.MultiThreadsCount
        };
    }
}
