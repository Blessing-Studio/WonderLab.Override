using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Classes.Models.Download;

namespace WonderLab.Classes.Handlers {
    /// <summary>
    /// 下载处理器
    /// </summary>
    public class DownloadHandler {
        private readonly BatchDownloader _downloader = new();

        public async ValueTask<BatchDownloader> DownloadAsync(DownloadRequest request) {
            _downloader.Setup(Enumerable.Repeat<DownloadRequest>(request, 1));
            await _downloader.DownloadAsync();
            return _downloader;
        }

        public async ValueTask<BatchDownloader> BatchDownloadAsync(IEnumerable<DownloadRequest> requests) {
            _downloader.Setup(requests);
            await _downloader.DownloadAsync();
            return _downloader;
        }
    }
}
