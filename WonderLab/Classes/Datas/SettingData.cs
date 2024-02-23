using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Game;

namespace WonderLab.Classes.Datas;

/// <summary>
/// 启动器的设置项数据模型
/// </summary>
public sealed record SettingData {
    [JsonPropertyName("activeJava")]
    public JavaEntry ActiveJava { get; set; }

    [JsonPropertyName("activeAccount")]
    public Account ActiveAccount { get; set; }
    
    [JsonPropertyName("activeGameId")]
    public string ActiveGameId { get; set; }
    
    [JsonPropertyName("activeGameFolder")]
    public string ActiveGameFolder { get; set; }
    
    [JsonPropertyName("javas")]
    public ObservableCollection<JavaEntry> Javas { get; set; }
    
    [JsonPropertyName("accounts")]
    public ObservableCollection<Account> Accounts { get; set; }
    
    [JsonPropertyName("gameFolders")]
    public ObservableCollection<string> GameFolders { get; set; }
}