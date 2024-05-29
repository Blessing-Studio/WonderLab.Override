using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Game;

namespace WonderLab.Classes.Datas;

/// <summary>
/// 启动器的设置项数据模型
/// </summary>
public sealed record SettingData {
    [JsonPropertyName("activeJava")] public JavaEntry ActiveJava { get; set; }
    [JsonPropertyName("activeAccount")] public Account ActiveAccount { get; set; }
    [JsonPropertyName("maxMemory")] public int MaxMemory { get; set; } = 1024;
    [JsonPropertyName("themeIndex")] public int ThemeIndex { get; set; } = 0;
    [JsonPropertyName("multiPartsCount")] public int MultiPartsCount { get; set; } = 0;
    [JsonPropertyName("multiThreadsCount")] public int MultiThreadsCount { get; set; } = 0;
    [JsonPropertyName("languageIndex")] public int LanguageIndex { get; set; } = 0;
    [JsonPropertyName("backgroundIndex")] public int BackgroundIndex { get; set; } = 0;
    [JsonPropertyName("testUserUuid")] public string TestUserUuid { get; set; }
    [JsonPropertyName("activeGameId")] public string ActiveGameId { get; set; }
    [JsonPropertyName("activeGameFolder")] public string ActiveGameFolder { get; set; }
    [JsonPropertyName("isFullScreen")] public bool IsFullScreen { get; set; }
    [JsonPropertyName("isGameIndependent")] public bool IsGameIndependent { get; set; }
    [JsonPropertyName("isAutoAllocateMemory")] public bool IsAutoAllocateMemory { get; set; }
    [JsonPropertyName("isAutoSelectJava")] public bool IsAutoSelectJava { get; set; }
    [JsonPropertyName("javas")] public ObservableCollection<JavaEntry> Javas { get; set; } = [];
    [JsonPropertyName("accounts")] public ObservableCollection<Account> Accounts { get; set; } = [];
    [JsonPropertyName("gameFolders")] public ObservableCollection<string> GameFolders { get; set; } = [];
}

[JsonSerializable(typeof(SettingData))]
sealed partial class SettingDataContext : JsonSerializerContext;