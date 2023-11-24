﻿using System;
using Avalonia;
using Avalonia.ReactiveUI;
using WonderLab.Classes.Utilities;

namespace WonderLab;

class Program {
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
