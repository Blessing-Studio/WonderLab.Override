using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages
{
    public class DownCenterPageViewModel : ReactiveObject {
        public DownCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {       
            
        }

        public CurseForgeToolkit Toolkit { get; } = new("$2a$10$Awb53b9gSOIJJkdV3Zrgp.CyFP.dI13QKbWn/4UZI4G4ff18WneB6");

        [Reactive]
        public ObservableCollection<WebModpackViewData> Modpacks { get; set; } = new();

        [Reactive]
        public bool IsLoading { get; set; }

        [Reactive]
        public bool IsSearchOptionsPaneOpen { get; set; } = false;

        public List<string> McVersions { get; } = new() {       
            "All",
            "1.19.4",
            "1.19.3",
            "1.19.2",
            "1.19.1",
            "1.19",
            "1.18.2",
            "1.17.1",
            "1.16.5",
            "1.15.2",
            "1.14.4",
            "1.13.2",
            "1.12.2",
            "1.11.2",
            "1.10.2",
            "1.9.4",
            "1.8.9",
            "1.7.10"
        };

        public Dictionary<int, string> ModpackCategories { get; } = new() {
            { 406, "世界生成" },
            { 407, "生物群系" },
            { 409, "天然结构" },
            { 410, "维度" },
            { 411, "生物" },
            { 412, "科技" },
            { 414, "交通与移动" },
            { 415, "管道与物流" },
            { 417, "能源" },
            { 4558, "红石" },
            { 420, "仓储" },
            { 416, "农业" },
            { 436, "食物" },
            { 419, "魔法" },
            { 434, "装备与工具" },
            { 422, "冒险与探索" },
            { 424, "建筑与装饰" },
            { 423, "信息显示" },
            { 425, "杂项" },
            { 421, "支持库" },
            { 435, "服务器" },
        };

        public async ValueTask GetModrinthModpackAsync() {
            Modpacks.Clear();

            var modpacks = await Task.Run(async () => await ModrinthToolkit.GetFeaturedModpacksAsync());
            foreach (var i in modpacks.Hits.AsParallel()) {
                await Task.Run(async () => {
                    var infos = await ModrinthToolkit.GetProjectInfos(i.ProjectId);
                    Modpacks.Add(new WebModpackModel(i, infos).CreateViewData<WebModpackModel, WebModpackViewData>());
                });
            }

            IsLoading = false;
        }

        public async ValueTask GetCurseforgeModpacksAsync() {
            Modpacks.Clear();

            var modpacks = await Task.Run(async () => await Toolkit.GetFeaturedModpacksAsync());
            foreach (var x in modpacks) {
                Modpacks.Add(new WebModpackModel(x).CreateViewData<WebModpackModel, WebModpackViewData>());
                await Task.Delay(10);
            }

            IsLoading = false;
        }

        public void OpenGameInstallDialogAction() {
            MainWindow.Instance.Install.InstallDialog.ShowDialog();
        }

        public async void GetModrinthModpackAction() {
            IsLoading = true;
            await GetModrinthModpackAsync();
        }

        public async void GetCurseforgeModpackAction() {       
            IsLoading = true;
            await GetCurseforgeModpacksAsync();
        }

        public void OpenSearchOptionsAction() {
            if (IsSearchOptionsPaneOpen) {
                IsSearchOptionsPaneOpen = false;
            }

            IsSearchOptionsPaneOpen = true;
        }

        public void GoBackAction() {
            MainWindow.Instance.NavigationPage(new ActionCenterPage());
        }
    }
}
