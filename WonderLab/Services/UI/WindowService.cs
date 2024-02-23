using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using WonderLab.Views.Windows;

namespace WonderLab.Services.UI;

/// <summary>
/// 主窗体 <see cref="MainWindow"/> 的扩展服务类
/// </summary>
public sealed class WindowService(MainWindow window) {
    private readonly MainWindow _mainWindow = window;

    public void Close() {
        _mainWindow.Close();
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