using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DynamicData;
using MinecraftLaunch.Modules.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class InstallerPage : UserControl {
        public static InstallerPageViewModel ViewModel { get; set; }

        public InstallerPage() {
            InitializeComponent();
        }

        private void OnModLoaderTypeSelectClick(object sender, RoutedEventArgs e) {
            ViewModel.CurrentLoaders?.Clear();
            GC.Collect();
            string tag = (sender as Button).Tag
                .ToString();

            var cache = tag switch {
                "Quilt" => CacheResources.Quilts,
                "Forge" => CacheResources.Forges,
                "Fabric" => CacheResources.Fabrics,
                "OptiFine" => CacheResources.Optifines,
                "NeoForge" => CacheResources.NeoForges,
                _ => new(),
            };

            ViewModel.CurrentLoaders = new(cache
                .ToList());

            "Loaded".ShowLog();
        }

        private void OnModLoaderSelectClick(object sender, RoutedEventArgs e) {
            ViewModel.GobackAction();
            var data = (sender as Button).DataContext as ModLoaderModel;
            var button = data.ModLoaderType switch {
                ModLoaderType.Quilt => RemoveQuiltButton,
                ModLoaderType.Forge => RemoveForgeButton,
                ModLoaderType.Fabric => RemoveFabricButton,
                ModLoaderType.NeoForged => RemoveNeoForgeButton,
                ModLoaderType.OptiFine => RemoveOptiFineButton,
                _ => default,
            };

            if (ViewModel.SelectLoaders.Any(x => x.ModLoaderType == data.ModLoaderType)) {
                ViewModel.SelectLoaders.Remove(button?.Tag as ModLoaderModel);
            }

            button.Tag = data;
            button.IsVisible = true;
            ViewModel.SelectLoaders?.Add(data);
        }

        private void OnRemoveLoaderClick(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if (ViewModel.SelectLoaders.Remove(button.Tag as ModLoaderModel)) {
                button.IsVisible = false;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            ViewModel = new(ModLoaderSelectPanel, ModLoadersSelectPanel);
            DataContext = ViewModel;
        }
    }
}
