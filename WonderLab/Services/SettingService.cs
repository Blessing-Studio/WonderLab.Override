using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using WonderLab.Classes.Datas;

namespace WonderLab.Services;

/// <summary>
/// 设置项服务类
/// </summary>
/// <remarks>
/// 用于管理启动器产生的持久化数据
/// </remarks>
public sealed class SettingService {
    private readonly FileInfo _settingDataFilePath = new(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "wonderlab", "settingData.json"));

    public SettingData Data { get; private set; }

    public SettingService() => Initialize();

    public void Save() {
        File.WriteAllText(_settingDataFilePath.FullName, Data.Serialize(typeof(SettingData),
            new SettingDataContext(JsonConverterUtil.DefaultJsonOptions)));
    }

    public void Initialize() {
        if (!_settingDataFilePath.Directory!.Exists) {
            _settingDataFilePath.Directory.Create();
        }

        if (_settingDataFilePath.Exists) {
            Data = File.ReadAllText(_settingDataFilePath.FullName).Deserialize(SettingDataContext.Default.SettingData);
        } else {
            Data = new();
            Save();
        }
    }
}