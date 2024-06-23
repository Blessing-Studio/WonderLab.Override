using System;
using Avalonia;
using System.Linq;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using WonderLab.Views.Windows;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.ViewModels.Windows;
using System.Collections.Generic;

namespace WonderLab.Services.UI;

/// <summary>
/// 主窗体 <see cref="MainWindow"/> 的扩展服务类
/// </summary>
public sealed class WindowService {
    private DialogService _dialogService;

    private readonly Window _mainWindow;
    private readonly LogService _logService;
    private readonly SettingService _settingService;

    public double ActualWidth => _mainWindow.Bounds.Width;
    public double ActualHeight => _mainWindow.Bounds.Height;

    public WindowService(SettingService settingService, LogService logService) {
        _logService = logService;
        _settingService = settingService;

        _mainWindow = _settingService.IsInitialize 
            ? App.ServiceProvider.GetService<OobeWindow>() 
            : App.ServiceProvider.GetService<MainWindow>();

        _mainWindow.ActualThemeVariantChanged += (_, args) => {
            if (_mainWindow.TransparencyLevelHint.Any(x => x == WindowTransparencyLevel.AcrylicBlur)) {
                SetBackground(1);
            }
        };
    }

    public void Close() {
        _mainWindow.Close();
        _logService.Finish();
    }

    public async void CopyText(string text) {
        await _mainWindow.Clipboard.SetTextAsync(text);
    }

    public async void SetBackground(int type) {
        var main = _mainWindow as MainWindow;
        main.Background = Brushes.Transparent;
        main.AcrylicMaterial.IsVisible = false;

        switch (type) {
            case 0:
                main.TransparencyLevelHint = [WindowTransparencyLevel.Mica];
                break;
            case 1:
                main.AcrylicMaterial.IsVisible = true;
                main.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                var tintColor = main.ActualThemeVariant == ThemeVariant.Dark ? Colors.Black : Colors.White;

                main.AcrylicMaterial.Material = new() {
                    TintOpacity = 0.5d,
                    TintColor = tintColor,
                    MaterialOpacity = 0.4d,
                    BackgroundSource = AcrylicBackgroundSource.Digger
                };
                break;
            case 2:
                string path = _settingService.Data.ImagePath;
                if (string.IsNullOrEmpty(path)) {
                    _dialogService = App.ServiceProvider.GetService<DialogService>();

                    var result = await _dialogService.OpenFilePickerAsync([
                        new FilePickerFileType("图像文件") { Patterns = new List<string>() { "*.png", "*.jpg", "*.jpeg", "*.tif", "*.tiff" } }
                    ], "打开文件");

                    if (result is null) {
                        return;
                    }

                    path = result.FullName;
                }

                //_settingService.Data.ImagePath = path;
                (main.DataContext as MainWindowViewModel).ImagePath = path;
                break;
        }
    }

    public void SetWindowState(WindowState state) {
        _mainWindow.WindowState = state;
    }

    public void BeginMoveDrag(PointerPressedEventArgs args) {
        _mainWindow.BeginMoveDrag(args);
    }

    public void HandlePropertyChanged(AvaloniaProperty property, Action action) {
        _mainWindow.PropertyChanged += (_, args) => {
            if (args.Property == property) {
                action?.Invoke();
            }
        };
    }

    public IStorageProvider GetStorageProvider() {
        return _mainWindow.StorageProvider;
    }
}