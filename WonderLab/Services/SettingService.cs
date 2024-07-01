using System;
using System.IO;
using System.Threading;
using WonderLab.Services.UI;
using System.Threading.Tasks;
using WonderLab.Classes.Datas;
using MinecraftLaunch.Utilities;
using MinecraftLaunch.Extensions;
using Microsoft.Extensions.Hosting;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using Avalonia.Threading;

namespace WonderLab.Services;

/// <summary>
/// 设置项服务类
/// </summary>
/// <remarks>
/// 用于管理启动器产生的持久化数据
/// </remarks>
public sealed class SettingService {
    public SettingData Data { get; private set; }
    public bool IsInitialize { get; private set; }

    public SettingService(WeakReferenceMessenger weakReferenceMessenger) {
        weakReferenceMessenger.Register<SettingDataChangedMessage>(this, (_, args) => {
            Data = args.Data;
        });

        weakReferenceMessenger.Register<IsDataInitializeChangedMessage>(this, (_, args) => {
            IsInitialize = args.IsInitialize;
        });
    }
}

internal sealed class SettingBackgroundService : BackgroundService {
    private bool _isInitialize;
    private SettingData _settingData;

    private readonly LogService _logService;
    private readonly ThemeService _themeService;
    private readonly WindowService _windowService;
    private readonly LanguageService _languageService;

    private readonly Dispatcher _dispatcher;
    private readonly FileInfo _settingDataFilePath;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    public SettingBackgroundService(
        Dispatcher dispatcher,
        LogService logService,
        ThemeService themeService,
        WindowService windowService,
        LanguageService languageService,
        WeakReferenceMessenger weakReferenceMessenger) {
        _logService = logService;
        _themeService = themeService;
        _windowService = windowService;
        _languageService = languageService;

        _dispatcher = dispatcher;
        _weakReferenceMessenger = weakReferenceMessenger;

        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _settingDataFilePath = new(Path
            .Combine(documentsPath, "Blessing-Studio", "wonderlab", "settingData.json"));
    }

    private void Save() {
        var json = _settingData.Serialize(typeof(SettingData), new SettingDataContext(JsonConverterUtil.DefaultJsonOptions));
        _logService.Debug(nameof(SettingService), $"开始保存数据，序列化结果如下：\n{json}");
        File.WriteAllText(_settingDataFilePath.FullName, json);
    }

    private void Initialize() {
        _logService.Info(nameof(SettingBackgroundService), "开始初始化设置数据服务");

        if (!_settingDataFilePath.Directory!.Exists) {
            _settingDataFilePath.Directory.Create();
        }

        if (_settingDataFilePath.Exists) {
            _settingData = File.ReadAllText(_settingDataFilePath.FullName).AsJsonEntry<SettingData>();
        } else {
            _settingData = new();
            Save();
        }

        _isInitialize = !_settingDataFilePath.Exists;
        _logService.Info(nameof(SettingBackgroundService), $"是否需要进入 OOBE：{_isInitialize}");

        _weakReferenceMessenger.Send(new SettingDataChangedMessage(_settingData));
        _weakReferenceMessenger.Send(new IsDataInitializeChangedMessage(_isInitialize));
        _dispatcher.Post(ViewDataInitialize);
    }

    private void ViewDataInitialize() {
        if (_isInitialize) {
            return;
        }

        _themeService.SetCurrentTheme(_settingData.ThemeIndex);
        _languageService.SetLanguage(_settingData.LanguageIndex);
        _windowService.SetBackground(_settingData.BackgroundIndex);
    }

    public override async Task StopAsync(CancellationToken cancellationToken) {
        await Task.Run(() => Save(), cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await Task.Run(Initialize, stoppingToken);
    }
}