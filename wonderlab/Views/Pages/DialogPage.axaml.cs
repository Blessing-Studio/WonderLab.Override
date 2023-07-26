using Avalonia.Controls;
using Avalonia.Threading;
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
    public partial class DialogPage : UserControl {
        public static DialogPageViewModel ViewModel { get; set; } = new();

        public DialogPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void ShowInfoDialog(string title, string message) {
            MainDialog.Title = title;
            MainDialog.Message = message;
            MainDialog.ShowDialog();
        }

        private async void OnSelectModLoaderChanged(object? sender, SelectModLoaderChangedArgs args) {
            await Dispatcher.UIThread.InvokeAsync(() => {
                switch (args.ModLoaderName) {
                    case "Forge":
                        InstallDialog.ModLoaders = CacheResources.Forges.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection();
                        break;
                    case "Fabric":
                        InstallDialog.ModLoaders = CacheResources.Fabrics.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection();
                        break;
                    case "Optifine":
                        InstallDialog.ModLoaders = CacheResources.Optifines.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection();
                        break;
                    case "Quilt":
                        InstallDialog.ModLoaders = CacheResources.Quilts.Select(x => {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";
                            return data;
                        }).ToObservableCollection();
                        break;
                }
            });
        }
    }
}
