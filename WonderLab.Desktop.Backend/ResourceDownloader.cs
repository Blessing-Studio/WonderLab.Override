using MinecraftLaunch;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Components.Checker;
using MinecraftLaunch.Components.Resolver;
using MinecraftLaunch.Classes.Models.Download;

namespace WonderLab.Desktop.Backend;
public static class ResourceDownloader {
    public static async Task CompleteResourceAsync(string path, string id, string source, int thread) {
        var gameResolver = new GameResolver(path);
        var checker = new ResourceChecker(gameResolver.GetGameEntity(id));
        bool isNeedComplete = !await checker.CheckAsync();
        Console.WriteLine($"{isNeedComplete}");
        Console.WriteLine($"{checker.MissingResources.Count}");

        if (isNeedComplete) {
            await checker.MissingResources.DownloadResourceEntrysAsync(GetSource(source), x => {
                Console.WriteLine($"[{x.CompletedCount}/{x.TotalCount}][{x.ToPercentage() * 100:0.00}%]");
            }, new() {
                MultiPartsCount = 8,
                MultiThreadsCount = thread,
                FileSizeThreshold = 1024 * 1024 * 3,
                IsPartialContentSupported = true
            });
        }
    }

    private static MirrorDownloadSource GetSource(string source) {
        if (source == "bmcl") {
            return MirrorDownloadManager.Bmcl;
        } else {
            return default!;
        }
    }
}