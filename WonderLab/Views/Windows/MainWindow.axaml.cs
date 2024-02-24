using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using System;
using WonderLab.Services.UI;

namespace WonderLab.Views.Windows;

public partial class MainWindow : Window {
    public MainWindow() => InitializeComponent();
    private new void Loaded(object? _, RoutedEventArgs e)
    {
        CloseButton.Click += (_, _) => Close();
        MinimizedButton.Click += (_, _) => WindowState = WindowState.Minimized;
        Layout.PointerPressed += (_, args) => BeginMoveDrag(args);
    }
}
