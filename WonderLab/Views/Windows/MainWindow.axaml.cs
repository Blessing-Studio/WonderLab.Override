using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Handlers;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Controls;
using WonderLab.Views.Pages;

namespace WonderLab.Views.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
    public MainWindow(MainWindowViewModel vm) {
        InitializeComponent();
        ViewModel = vm;
    }

    public MainWindow() {
        InitializeComponent(); 
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (Design.IsDesignMode) {
            return;
        }

        ViewModel.Init();
    }
}
