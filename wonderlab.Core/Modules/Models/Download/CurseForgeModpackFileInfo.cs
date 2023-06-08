using MinecraftLaunch.Modules.Enum;
using Newtonsoft.Json;

namespace MinecraftLaunch.Modules.Models.Download;

public class CurseForgeModpackFileInfo
{
	public string DownloadUrl { get; set; }

	[JsonProperty("fileId")]
	public int FileId { get; set; }

	[JsonProperty("filename")]
	public string FileName { get; set; }

	[JsonProperty("modLoader")]
	public ModLoaderType? ModLoaderType { get; set; }

	[JsonProperty("gameVersion")]
	public string SupportedVersion { get; set; }

    [JsonProperty("dependencies")]
    public List<Dependencie> Dependencies { get; set; }
}

public class Dependencie {
    [JsonProperty("modId")]
    public long ModId { get; set; }

    [JsonProperty("relationType")]
    public long RelationType { get; set; }
}