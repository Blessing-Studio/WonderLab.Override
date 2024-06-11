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
    private readonly LogService _logService;
    private readonly FileInfo _settingDataFilePath = new(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Blessing-Studio", "wonderlab", "settingData.json"));

    public SettingData Data { get; private set; }
    public bool IsInitialize { get; private set; }

    public SettingService(LogService logService) {
        _logService = logService;
        Initialize();
    }

    public void Save() {
        var json = Data.Serialize(typeof(SettingData), new SettingDataContext(JsonConverterUtil.DefaultJsonOptions));
        _logService.Debug(nameof(SettingService), $"开始保存数据，序列化结果如下：\n{json}");
        File.WriteAllText(_settingDataFilePath.FullName, json);
    }

    public void Initialize() {
        _logService.Info(nameof(SettingService), "开始初始化设置数据服务");

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
        _logService.Info(nameof(SettingService), $"是否需要进入 OOBE：{IsInitialize}");
    }
}