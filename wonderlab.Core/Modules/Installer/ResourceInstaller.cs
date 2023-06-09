using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Parser;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.IO;
using Natsurainko.Toolkits.Network;
using Natsurainko.Toolkits.Network.Downloader;
using Natsurainko.Toolkits.Network.Model;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace MinecraftLaunch.Modules.Installer;

public class ResourceInstaller {
    public GameCore GameCore { get; set; }

    public List<IResource> FailedResources { get; set; } = new List<IResource>();

    public static int MaxDownloadThreads { get; set; } = 64;

    public async ValueTask<ResourceInstallResponse> DownloadAsync(Action<string, float> func) {
        var resources = new List<IResource>();
        resources.AddRange(GameCore.LibraryResources!.AsParallel().Where(x => x.IsEnable));
        resources.AddRange(GetFileResources());
        resources.AddRange(await GetAssetResourcesAsync());

        resources = resources.AsParallel().Where(x => {
            if (string.IsNullOrEmpty(x.CheckSum) && x.Size == 0)
                return false;
            if (x.ToFileInfo().Verify(x.CheckSum) && x.ToFileInfo().Verify(x.Size))
                return false;

            return true;
        }).ToList();

        int output = 0;
        TransformManyBlock<List<IResource>, IResource> manyBlock = new(x => x);
        ActionBlock<IResource> block = new(async x => {
            var request = x.ToDownloadRequest();

            if (!request.Directory.Exists)
                request.Directory.Create();

            func($"{output}/{resources.Count}", output / (float)resources.Count);

            Trace.WriteLine($"资源链接：{request.Url}");
            var result = await HttpToolkit.HttpDownloadAsync(request);
            output++;
            if (result.HttpStatusCode != HttpStatusCode.OK)
                this.FailedResources.Add(x);
        }, new ExecutionDataflowBlockOptions {
            BoundedCapacity = MaxDownloadThreads,
            MaxDegreeOfParallelism = MaxDownloadThreads
        });

        DataflowLinkOptions linkOptions = new DataflowLinkOptions {
            PropagateCompletion = true
        };
        using var disposable = manyBlock.LinkTo(block, linkOptions);
        manyBlock.Post(resources);
        manyBlock.Complete();
        await block.Completion;

        return new ResourceInstallResponse {
            FailedResources = this.FailedResources,
            SuccessCount = 6 - this.FailedResources.Count,
            Total = 6
        };
    }

    public IEnumerable<IResource> GetFileResources() {
        if (GameCore.ClientFile != null)
            yield return GameCore.ClientFile;
    }

    public async static ValueTask<List<IResource>> GetAssetFilesAsync(GameCore core) {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var asset = new AssetParser(new AssetJsonEntity().FromJson(await File.ReadAllTextAsync(core.AssetIndexFile.ToFileInfo().FullName)), core.Root).GetAssets().Select((Func<AssetResource, IResource>)((AssetResource x) => x)).ToList();
        List<IResource> resources = new List<IResource?>();
        if (core.LibraryResources != null)
        {
            var res = core.LibraryResources!.Where((LibraryResource x) => x.IsEnable).Select((Func<LibraryResource, IResource>)((LibraryResource x) => x)).ToList();
            resources.AddRange(res);
        }
        resources.AddRange(asset);
        resources.Sort((x, x1) => x.Size.CompareTo(x1.Size));

        foreach (var i in asset) {
            if (File.Exists(Path.Combine(Path.Combine(root, i.ToDownloadRequest().Directory.FullName.Substring(i.ToDownloadRequest().Directory.FullName.IndexOf(".minecraft"))), i.ToDownloadRequest().FileName))) {
                Console.WriteLine("文件 {0} 存在在官方目录！", i.ToDownloadRequest().FileName);
            }
        }

        return resources;
    }

    public async Task<List<IResource>> GetAssetResourcesAsync() {
        if (!(GameCore.AssetIndexFile.FileInfo.Verify(GameCore.AssetIndexFile.Size)
            || GameCore.AssetIndexFile.FileInfo.Verify(GameCore.AssetIndexFile.CheckSum))) {
            var request = GameCore.AssetIndexFile.ToDownloadRequest();

            if (!request.Directory.Exists)
                request.Directory.Create();

            var res = await HttpWrapper.HttpDownloadAsync(request);
            if(res.FileInfo == null){
                return new();
            }
            if (!res.FileInfo.Exists)
                return new();
        }
        
        var entity = JsonConvert.DeserializeObject<AssetJsonEntity>
            (await File.ReadAllTextAsync(GameCore.AssetIndexFile.ToFileInfo()!.FullName));

        return new AssetParser(entity!, GameCore.Root!).GetAssets().Select(x => (IResource)x).ToList();
    }

    [Obsolete]
    public IEnumerable<IResource> GetModLoaderResourcesAsync() {
        var entity = new GameCoreJsonEntity().FromJson(File.ReadAllText(Path.Combine(this.GameCore.Root.FullName, "versions", this.GameCore.Id, $"{this.GameCore.Id}.json")));
        List<LibraryResource> list = entity.Libraries.Select(x => {
            return new LibraryResource() {
                Root = GameCore.Root,
                Name = x.Name,
                Size = x.Name.Length,
                CheckSum = "114514",
                Url = x.Url,
            };
        }).ToList();

        foreach (var i in list) {
            if (!i.ToFileInfo().Exists) {
                yield return i;
            }
        }
    }

    public ResourceInstaller(GameCore core) {
        GameCore = core;
    }
}
