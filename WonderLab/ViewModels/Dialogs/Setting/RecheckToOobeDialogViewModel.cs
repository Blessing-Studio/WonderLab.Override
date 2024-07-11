using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.IO;
using Avalonia;
using WonderLab;
using WonderLab.Classes.Datas;
using WonderLab.Services.UI;
using WonderLab.Utilities;
using WonderLab.Views.Dialogs.Setting;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Views.Windows;
using WonderLab.ViewModels.Windows;
using WonderLab.Services;
using System;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics;

namespace WonderLab.ViewModels.Dialogs.Setting;
public sealed partial class RecheckToOobeDialogViewModel : DialogViewModelBase {
    private readonly WindowService _windowService;
    private readonly SettingService _settingService;
    private readonly ILogger<RecheckToOobeDialog> _logger;

    private readonly FileInfo _settingDataFilePath;
    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public RecheckToOobeDialogViewModel(
        WindowService windowService,
        SettingService settingService,
        ILogger<RecheckToOobeDialog> logger) {
        _logger = logger;
        _windowService = windowService;
        _settingService = settingService;

        _settingDataFilePath = new(Path
            .Combine(documentsPath, "Blessing-Studio", "wonderlab", "settingData.json"));
    }
    
    [RelayCommand]
    private void ToOobe() {
        _logger.LogInformation("开始初始化配置");
        File.Delete(_settingDataFilePath.FullName);
    }
}