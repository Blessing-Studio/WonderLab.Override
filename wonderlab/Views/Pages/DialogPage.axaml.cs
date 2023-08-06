using Avalonia.Controls;
using Avalonia.Threading;
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

        public DialogPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void ShowInfoDialog(string title, string message) {
            Dispatcher.UIThread.Post(() => {
                MainDialog.Title = title;
                MainDialog.Message = message;
                MainDialog.ShowDialog();
            });
        }

        private void OnSelectModLoaderChanged(object? sender, SelectModLoaderChangedArgs args) {
            ObservableCollection<ModLoaderViewData> modLoaders = new();
            InstallDialog.ModLoaders = modLoaders;
            Dispatcher.UIThread.Post(() => {
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
            }, DispatcherPriority.Background);
        }
    }
}
