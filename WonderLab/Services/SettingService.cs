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
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.ApplicationInsights;

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

    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<SettingBackgroundService> _logger;

    private readonly ThemeService _themeService;
    private readonly WindowService _windowService;
    private readonly LanguageService _languageService;

    private readonly Dispatcher _dispatcher;
    private readonly FileInfo _settingDataFilePath;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    public SettingBackgroundService(
        Dispatcher dispatcher,
        ThemeService themeService,
        WindowService windowService,
        TelemetryClient telemetryClient,
        LanguageService languageService,
        ILogger<SettingBackgroundService> logger,
        WeakReferenceMessenger weakReferenceMessenger) {
        _logger = logger;
        _themeService = themeService;
        _windowService = windowService;
        _languageService = languageService;

        _dispatcher = dispatcher;
        _telemetryClient = telemetryClient;
        _weakReferenceMessenger = weakReferenceMessenger;

        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _settingDataFilePath = new(Path
            .Combine(documentsPath, "Blessing-Studio", "wonderlab", "settingData.json"));
    }

    private void Save() {
        _telemetryClient.Flush();
        var json = _settingData.Serialize(typeof(SettingData), new SettingDataContext(JsonConverterUtil.DefaultJsonOptions));
        File.WriteAllText(_settingDataFilePath.FullName, json);
    }

    private void Initialize() {
        _logger.LogInformation("开始初始化设置数据服务");

        if (!_settingDataFilePath.Directory!.Exists) {
            _settingDataFilePath.Directory.Create();
        }

        _isInitialize = !_settingDataFilePath.Exists;

        if (_settingDataFilePath.Exists) {
            try {
                _settingData = File.ReadAllText(_settingDataFilePath.FullName).AsJsonEntry<SettingData>();
            } catch (JsonException) {
                _logger.LogError("Json 序列化时出现故障，开始重置设置");
                _settingData = new();
                Save();

                _isInitialize = true;
            }
        } else {
            _settingData = new();
            Save();
        }

        _logger.LogInformation("是否需要进入 OOBE：{IsOOBE}", _isInitialize);

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