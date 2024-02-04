using MinecraftLaunch.Classes.Models.Game;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using WonderLab.Classes.Enums;

namespace WonderLab.Classes.Models;

public class ConfigDataModel
{
    [JsonPropertyName("isFullscreen")]
    public bool IsFullscreen { get; set; }

    [JsonPropertyName("backgroundType")]
    public BackgroundType BackgroundType { get; set; }

    [JsonPropertyName("javaPath")]
    public JavaEntry JavaPath { get; set; }

    [JsonPropertyName("isAutoSelectJava")]
    public bool IsAutoSelectJava { get; set; }

    [JsonPropertyName("isAutoMemory")]
    public bool IsAutoMemory { get; set; }

    [JsonPropertyName("maxMemory")]
    public int MaxMemory { get; set; } = 1024;

    [JsonPropertyName("width")]
    public int Width { get; set; } = 854;

    [JsonPropertyName("height")]
    public int Height { get; set; } = 480;

    [JsonPropertyName("gameFolder")]
    public string GameFolder { get; set; }

    [JsonPropertyName("currentGameCoreId")]
    public string CurrentGameCoreId { get; set; }

    [JsonPropertyName("isEnableIndependencyCore")]
    public bool IsEnableIndependencyCore { get; set; }

    [JsonPropertyName("branch")]
    public BranchType Branch { get; set; } = BranchType.Lsaac;

    [JsonPropertyName("javaPaths")]
    public ObservableCollection<JavaEntry> JavaPaths { get; set; } = new();

    [JsonPropertyName("gameFolders")]
    public ObservableCollection<string> GameFolders { get; set; } = new();
}
