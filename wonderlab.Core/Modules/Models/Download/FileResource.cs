using System.Diagnostics;
using System.IO;
using MinecraftLaunch.Modules.Interface;
using Natsurainko.Toolkits.Network;
using Natsurainko.Toolkits.Network.Model;

namespace MinecraftLaunch.Modules.Models.Download;

public class FileResource : IResource {
    public DirectoryInfo? Root { get; set; }

    public string? Name { get; set; }

    public int Size { get; set; }

    public string? CheckSum { get; set; }

    public string? Url { get; set; }

    public FileInfo? FileInfo { get; set; }

    public HttpDownloadRequest ToDownloadRequest() {
        string url = string.Empty;
        if (Name.Contains("json")) {
            url = UrlExtension.Combine(APIManager.Current.Host,"v1",
                "packages",CheckSum,Name);
        }
        else {
            url = UrlExtension.Combine(APIManager.Current.Host,
            "v1", "objects", CheckSum, "client.jar");
        }

        return new HttpDownloadRequest {
            Directory = FileInfo!.Directory,
            FileName = Name,
            Sha1 = CheckSum,
            Size = Size,
            Url = url

        };
    }

    public FileInfo ToFileInfo() {
        return FileInfo!;
    }
}
