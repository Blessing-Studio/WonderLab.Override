using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.control;
using wonderlab.control.Controls.Bar;
using wonderlab.Views.Windows;
using MainWindow = wonderlab.Views.Windows.MainWindow;

namespace wonderlab {
    public partial class App : Application {
        public static MainWindow CurrentWindow { get; protected set; } = null!;

        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = CurrentWindow = new MainWindow();
                Manager.Current = CurrentWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}