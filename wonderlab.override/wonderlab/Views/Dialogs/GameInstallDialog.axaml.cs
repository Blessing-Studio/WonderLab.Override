using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.ViewModels.Dialogs;

namespace wonderlab.Views.Dialogs
{
    public partial class GameInstallDialog : UserControl
    {
        public static GameInstallDialogViewModel ViewModel { get; set; }
        public GameInstallDialog()
        {
            Initialized += this.DialogInitialized;
            InitializeComponent();
            DataContext = ViewModel = new();
        }

        private void DialogInitialized(object? sender, System.EventArgs e) {
            foreach (Button i in Installer.Children)
            {
                i.Click += (x, _) => {
                    var sender = x as Button;

                    ViewModel.ChangeTitle($"选择一个 {sender.Tag} 版本");
                    ViewModel.CurrentModLoader = null;
                    ViewModel.McVersionVisible = false;
                    ViewModel.ModLoaderVisible = true;

                    ViewModel.ModLoaders = sender.Tag switch
                    {
                        "Forge" => ViewModel.forges.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        "Fabric" => ViewModel.fabrics.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        "Optifine" => ViewModel.optifine.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        "Quilt" => ViewModel.quilt.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        _ => new()
                    };
                };

                if (i.Tag as string is "Optifine")
                {
                    i.PointerEnter += (_, _) => {
                        i.Margin = new(0, 0, -75, 0);
                    };

                    i.PointerLeave += (_, _) => {
                        i.Margin = new(0);
                    };

                    continue;
                }

                i.PointerEnter += (_, _) => {
                    i.Margin = new(0, 0, -60, 0);
                };

                i.PointerLeave += (_, _) => {
                    i.Margin = new(0);
                };
            }
        }

        private void RemoveModLoaderClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var modloader = (sender as Button)!.DataContext as ModLoaderViewData;
            ViewModel.CurrentModLoaders.Remove(modloader);
        }
    }
}
