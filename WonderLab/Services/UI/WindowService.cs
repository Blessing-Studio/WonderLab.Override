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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WonderLab.Views;
using System.Xml.Serialization;
using WonderLab.Services.Wrap;

namespace WonderLab.Services.UI;

/// <summary>
/// 主窗体 <see cref="MainWindow"/> 的扩展服务类
/// </summary>
public sealed class WindowService {
    private DialogService _dialogService;
    private Action<PointerEventArgs> _pointerMovedAction;
    private Action<PointerEventArgs> _pointerExitedAction;

    private static Window _mainWindow;

    private readonly WrapService _wrapService;
    private readonly SettingService _settingService;
    private readonly ILogger<WindowService> _logger;

    public bool IsLoaded => _mainWindow.IsLoaded;
    public double ActualWidth => _mainWindow.Bounds.Width;
    public double ActualHeight => _mainWindow.Bounds.Height;

    public WindowService(SettingService settingService, WrapService wrapService, ILogger<WindowService> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _settingService = settingService;

        _mainWindow = SettingService.IsInitialize
            ? App.ServiceProvider.GetService<OobeWindow>()
            : App.ServiceProvider.GetService<MainWindow>();

        _mainWindow.ActualThemeVariantChanged += (_, args) => {
            if (_mainWindow.TransparencyLevelHint.Any(x => x == WindowTransparencyLevel.AcrylicBlur)) {
                SetBackground(1);
            }
        };
    }

    public static void ChangeToOobe() {
        _mainWindow = App.ServiceProvider.GetService<OobeWindow>();
    }

    public void Close() {
        if (_wrapService.Client is { IsConnected:true }) {
            _wrapService.Close();
        }

        _mainWindow.Close();
    }

    public async void CopyText(string text) {
        await _mainWindow.Clipboard.SetTextAsync(text);
    }

    public async void SetBackground(int type) {
        var main = _mainWindow as MainWindow;
        if (main is null) {
            return;
        }

        main.Background = Brushes.Transparent;
        main.AcrylicMaterial.IsVisible = false;
        main.imageBox.IsVisible = false;

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
                main.imageBox.IsVisible = true;
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

                var mainVM = (main.DataContext as MainWindowViewModel);
                mainVM.ImagePath = path;
                mainVM.BlurRadius = _settingService.Data.BlurRadius;
                mainVM.IsEnableBlur = _settingService.Data.IsEnableBlur;
                _settingService.Data.ImagePath = path;
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

    public void RegisterPointerMoved(Action<PointerEventArgs> action) {
        _pointerMovedAction = action;
        _mainWindow.PointerMoved += OnPointerMoved;
    }

    public void RegisterPointerExited(Action<PointerEventArgs> action) {
        _pointerExitedAction = action;
        _mainWindow.PointerExited += OnPointerExited;
    }

    public void UnregisterPointerMoved() {
        _mainWindow.PointerMoved -= OnPointerMoved;
    }

    public void UnregisterPointerExited() {
        _mainWindow.PointerExited -= OnPointerExited;
    }

    public IStorageProvider GetStorageProvider() {
        return _mainWindow.StorageProvider;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e) {
        _pointerMovedAction?.Invoke(e);
    }

    private void OnPointerExited(object sender, PointerEventArgs e) {
        _pointerExitedAction?.Invoke(e);
    }
}