using Avalonia.Controls;
using Avalonia.Threading;
using MinecraftLaunch.Modules.Enum;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Views.Pages {
    public partial class DialogPage : UserControl {
        public static DialogPageViewModel ViewModel { get; set; } = new();
        public static List<ModLoaderViewData> CurrentModLoaders = new();

        public DialogPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void OnCurrentModLoaderChanged(object? sender, SelectModLoaderChangedArgs e) {
            var loader = (ModLoaderViewData)((ListBox)sender!).SelectedItem!;
            var loaders = CurrentModLoaders.ToList();
            if (!loader.IsNull()) {
                if (loaders.Count() == 0) {
                    CurrentModLoaders.Add(loader);
                    InstallDialog.CurrentModLoadersListBox.Items.Add(loader);
                }

                foreach (var x in loaders) {
                    if (x.Id != loader.Id && x.Data.ModLoader == loader.Data.ModLoader) {
                        //x = loader;
                    }

                    if ((x.Data.ModLoaderType == ModLoaderType.Forge || x.Data.ModLoaderType == ModLoaderType.OptiFine)
                        && (loader.Data.ModLoaderType == ModLoaderType.Fabric || loader.Data.ModLoaderType == ModLoaderType.Quilt)) {
                        "无法选择这个加载器，原因：加载器冲突！".ShowMessage();
                    }

                    if ((x.Data.ModLoaderType == ModLoaderType.Fabric || x.Data.ModLoaderType == ModLoaderType.Quilt)
                        && (loader.Data.ModLoaderType == ModLoaderType.OptiFine || loader.Data.ModLoaderType == ModLoaderType.Forge)) {
                        "无法选择这个加载器，原因：加载器冲突！".ShowMessage();
                    }

                    CurrentModLoaders.Add(loader);
                    InstallDialog.CurrentModLoadersListBox.Items.Add(loader);
                }
            }
        }

        public void ShowInfoDialog(string title, string message) {
            Dispatcher.UIThread.Post(() => {
                MainDialog.Title = title;
                MainDialog.Message = message;
                MainDialog.ShowDialog();
            });
        }

        private async void OnSelectModLoaderChanged(object? sender, SelectModLoaderChangedArgs args) {
            ObservableCollection<ModLoaderViewData> modLoaders = new();
            InstallDialog.ModLoaders = modLoaders;
            await Task.Delay(50);
            await Task.Run(() => {
                switch (args.ModLoaderName) {
                    case "Forge":
                        modLoaders.Load(CacheResources.Forges.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection());
                        break;
                    case "Fabric":
                        modLoaders.Load(CacheResources.Fabrics.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection());
                        break;
                    case "Optifine":
                        modLoaders.Load(CacheResources.Optifines.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection());
                        break;
                    case "Quilt":
                        modLoaders.Load(CacheResources.Quilts.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection());
                        break;
                }

            });
        }
    }
}
