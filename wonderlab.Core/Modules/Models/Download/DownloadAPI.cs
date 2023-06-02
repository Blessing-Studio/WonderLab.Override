namespace MinecraftLaunch.Modules.Models.Download;

public class DownloadAPI
{
	public string Host { get; init; }

	public string VersionManifest { get; init; }

	public string Assets { get; init; }

	public string Libraries { get; init; }

    public override bool Equals(object? obj) {
		var result = obj as DownloadAPI;
		return (Host == result.Host && VersionManifest == result.VersionManifest && Assets == result.Assets && Libraries == result.Libraries);
    }
}
