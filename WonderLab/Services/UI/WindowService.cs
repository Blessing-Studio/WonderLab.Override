using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using WonderLab.Views.Windows;
using Avalonia.Platform.Storage;
using WonderLab.Classes.Enums;
using Avalonia.Media;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using System.Linq;

namespace WonderLab.Services.UI;

/// <summary>
/// 主窗体 <see cref="MainWindow"/> 的扩展服务类
/// </summary>
public sealed class WindowService {
    private readonly MainWindow _mainWindow;

    public WindowService(MainWindow window) {
        _mainWindow = window;

        _mainWindow.ActualThemeVariantChanged += (_, args) => {
            if (_mainWindow.TransparencyLevelHint.Any(x => x == WindowTransparencyLevel.AcrylicBlur)) {
                SetBackground(1);
            }
        };
    }

    public void Close() {
        _mainWindow.Close();
    }

    public IStorageProvider GetStorageProvider() {
        return _mainWindow.StorageProvider;
    }

    public void SetBackground(int type) {
        _mainWindow.Background = Brushes.Transparent;
        _mainWindow.AcrylicMaterial.IsVisible = false;

        switch (type) {
            case 0:
                _mainWindow.TransparencyLevelHint = [WindowTransparencyLevel.Mica];
                break;
            case 1:
                _mainWindow.AcrylicMaterial.IsVisible = true;
                _mainWindow.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                var tintColor = _mainWindow.ActualThemeVariant == ThemeVariant.Dark ? Colors.Black : Colors.White;

                _mainWindow.AcrylicMaterial.Material = new() {
                    TintOpacity = 0.5d,
                    TintColor = tintColor,
                    MaterialOpacity = 0.4d,
                    BackgroundSource = AcrylicBackgroundSource.Digger
                };
                break;
            case 2:
                _mainWindow.Background = new ImageBrush {
                    Source = new Bitmap(""),
                    Stretch = Stretch.UniformToFill
                };
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

    public double ActualWidth => _mainWindow.Bounds.Width;

    public double ActualHeight => _mainWindow.Bounds.Height;
}