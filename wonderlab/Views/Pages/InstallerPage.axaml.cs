using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DynamicData;
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

        private async void OnModLoaderTypeSelectClick(object sender, RoutedEventArgs e) {
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

            IEnumerable<ModLoaderViewData> cachedatas = default;
            await Task.Factory.StartNew(() => {
                //cachedatas = cache.Select(x => x
                //    .CreateViewData<ModLoaderModel, ModLoaderViewData>());

                ViewModel.CurrentLoaders = new(cache
                    .ToList());

                "Loaded".ShowLog();
            });
        }

        private async void OnModLoaderSelectClick(object sender, RoutedEventArgs e) {
            ViewModel.GobackAction();
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            ViewModel = new(ModLoaderSelectPanel, ModLoadersSelectPanel);
            DataContext = ViewModel;
        }
    }
}
