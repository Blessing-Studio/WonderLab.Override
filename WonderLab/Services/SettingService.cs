using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using WonderLab.Classes.Datas;
using MinecraftLaunch.Utilities;
using MinecraftLaunch.Extensions;

namespace WonderLab.Services;

/// <summary>
/// 设置项服务类
/// </summary>
/// <remarks>
/// 用于管理启动器产生的持久化数据
/// </remarks>
public sealed class SettingService {
    private readonly FileInfo _settingDataFilePath = new(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Blessing-Studio", "wonderlab", "settingData.json"));

    public SettingData Data { get; private set; }
    public bool IsInitialize { get; private set; }

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
            Data = File.ReadAllText(_settingDataFilePath.FullName).AsJsonEntry<SettingData>();
        } else {
            Data = new();
            Save();
        }

        IsInitialize = !_settingDataFilePath.Exists;
    }
}