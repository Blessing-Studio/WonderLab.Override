using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using WonderLab.Classes.Models;
using MinecraftLaunch.Components.Fetcher;

namespace WonderLab.Services;

/// <summary>
/// 数据服务类
/// </summary>
public class DataService {
    private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    public string Version =>
        (Attribute.GetCustomAttribute(_assembly, typeof(AssemblyFileVersionAttribute), false)
            as AssemblyFileVersionAttribute)!.Version;

    public string FolderPath => Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData), "wonderlab");

    public string DataFilePath => Path.Combine(FolderPath, "data.json");

    public readonly JavaFetcher javaFetcher = new();
    
    public ConfigDataModel ConfigData { get; set; }

    public void Load() {
        if (File.Exists(DataFilePath)) {
            string json = File.ReadAllText(DataFilePath);
            ConfigData = JsonSerializer.Deserialize<ConfigDataModel>(json)!;
        } else {
            _ = SaveAsync();
        }
    }

    public async ValueTask SaveAsync() {
        var json = JsonSerializer.Serialize(ConfigData ?? new());
        await File.WriteAllTextAsync(DataFilePath, json);
    }
}