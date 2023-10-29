using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.control;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class InstallerPage : UserControl {
        public static InstallerPageViewModel ViewModel { get; set; }

        public InstallerPage() {
            InitializeComponent();
            new Button().Click += OnModLoaderSelectClick; ;
        }

        private void OnModLoaderSelectClick(object sender, RoutedEventArgs e) {
            var currentList = ViewModel.CurrentLoaders;
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

            currentList?.Clear();
            currentList?.Load(cache);
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            ViewModel = new(ModLoaderSelectPanel, ModLoadersSelectPanel);
            DataContext = ViewModel;
        }
    }
}
