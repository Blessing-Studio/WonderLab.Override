using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Launch;
using System;
using System.Collections.ObjectModel;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.ViewModels.Windows;

namespace wonderlab.Views.Windows
{
    public partial class ConsoleWindow : Window
    {
        public static bool IsOpen { get; set; } = false;
        public static ConsoleWindowViewModel ViewModel { get; set; } = new();

        private static ConsoleWindow Console { get; set; } = null;
        public ConsoleWindow() {       
            InitializeComponent();
            Console = this;

            DataContext = ViewModel = new();
            IsOpen = true;

            Closed += (_, _) => {           
                IsOpen = false;
                Console = null;
            };
        }

        public static void WindowActivate() => Console.Activate();

        private void OnPropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e) {
            $"{e.Property} - {e.NewValue}".ShowLog();
        }


    }

    public static class ProcessManager {
        public static ObservableCollection<Tuple<string, string>> History { get; set; } = new();

        public static ObservableCollection<MinecraftProcessViewData> GameCoreProcesses { get; set; } = new();
    }
}
