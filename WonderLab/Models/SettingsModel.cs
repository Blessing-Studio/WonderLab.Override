using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using WonderLab.Enums;

namespace WonderLab.Models;

public record SettingsModel {
    // Game Settings
    [JsonPropertyName("isolationVersion")] public bool IsolationVersion { get; set; }
    
    [JsonPropertyName("gameWindowType")] public GameWindowTypes GameWindowType { get; set; }
    
    [SupportedOSPlatform("windows")]
    [JsonPropertyName("minecraftPriority")]
    public ProcessPriorityClass MinecraftPriority { get; set; }
    
    // Appearance Settings 
    [JsonPropertyName("color")] public int Color { get; set; }
    [JsonPropertyName("isFollowSystem")] public bool IsFollowSystem { get; set; }
    
    // Network Settings
    [JsonPropertyName("maxThread")] public int MaxThread { get; set; }
    [JsonPropertyName("isEnableMirror")] public bool IsEnableMirrorDownloadSource { get; set; }
}