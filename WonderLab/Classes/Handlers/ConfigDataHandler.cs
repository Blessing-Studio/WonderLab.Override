using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WonderLab.Classes.Models;

namespace WonderLab.Classes.Handlers {
    public class ConfigDataHandler {
        public string FolderPath => Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "wonderlab");

        public string DataFilePath => Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "wonderlab", "data.json");

        public ConfigDataModel ConfigDataModel { get; set; }

        public void Load() {
            if (File.Exists(DataFilePath)) {
                string json = File.ReadAllText(DataFilePath);
                ConfigDataModel = JsonSerializer.Deserialize
                    <ConfigDataModel>(json)!;
            } else {
                _ = SaveAsync();
            }
        }

        public async ValueTask SaveAsync() {
            var json = JsonSerializer.Serialize(ConfigDataModel ?? new());
            await File.WriteAllTextAsync(DataFilePath, json);
        }
    }
}
