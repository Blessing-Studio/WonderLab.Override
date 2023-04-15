using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Toolkits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public ObservableCollection<WebModpackViewData> Resources { get; set; } = new();

        [Reactive]
        public bool IsLoading { get; set; }

        [Reactive]
        public string SearchFilter { get; set; }

        [Reactive]
        public string CurrentMcVersion { get; set; } = string.Empty;

        [Reactive]
        public KeyValuePair<int, string> CurrentCategorie { get; set; }

        [Reactive]
        public double SearcherHeight { get; set; } = 0;

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
        
        public Dictionary<int, string> Categories { get; } = new() {
            { 6, "模组" },
            { 4471, "整合包" },
            { 12, "资源包" },
            { 17, "地图" },
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

        public async ValueTask GetModrinthResourceAsync() {
            Resources.Clear();

            var modpacks = await Task.Run(async () => await ModrinthToolkit.GetFeaturedsAsync());
            foreach (var i in modpacks.Hits.AsParallel()) {
                await Task.Run(async () => {
                    var infos = await ModrinthToolkit.GetProjectInfos(i.ProjectId);
                    Resources.Add(new WebModpackModel(i, infos).CreateViewData<WebModpackModel, WebModpackViewData>());
                });
            }

            IsLoading = false;
        }

        public async ValueTask GetCurseforgeResourceAsync() {
            Resources.Clear();

            var modpacks = await Task.Run(async () => await Toolkit.GetFeaturedsAsync());
            foreach (var x in modpacks) {
                Resources.Add(new WebModpackModel(x).CreateViewData<WebModpackModel, WebModpackViewData>());
                await Task.Delay(10);
            }

            IsLoading = false;
        }

        public async ValueTask SearchCurseforgeResourceAsync() {
            try {
                //模组中文搜索检测
                var searchFilter = string.Empty;
                if (CurrentCategorie.Key == 6) {
                    foreach (var item in DataUtil.WebModpackInfoDatas.AsParallel()) {           
                        if(SearchFilter.IsChinese() && item.Value.Chinese.Contains(SearchFilter) && item.Value.Chinese.Contains("(") && item.Value.Chinese.Contains(")")) {
                            item.Value.CurseForgeId = item.Value.Chinese.Split(" (")[1].Split(")").First().Trim();
                            searchFilter = item.Value.CurseForgeId;
                            
                            Trace.WriteLine($"[信息] 新的 CurseForgeId 值为 {searchFilter}");
                            break;
                        } else if (SearchFilter.IsChinese()) searchFilter = item.Value.CurseForgeId.Replace("-", " ");
                    }
                }
                else {
                    searchFilter = SearchFilter;
                }


                Resources.Clear();
                var result = (await Toolkit.SearchResourceAsync(string.IsNullOrEmpty(searchFilter) ? SearchFilter : searchFilter, 
                    CurrentCategorie.Key, gameVersion: CurrentMcVersion.Contains("All") ? string.Empty : CurrentMcVersion))
                    .Select(x => new WebModpackModel(x).CreateViewData<WebModpackModel, WebModpackViewData>()).ToList();

                //重新排序
                var list = result.ToList();
                foreach (var item in list) {
                    if (SearchFilter.IsChinese() && item.Data.ChineseTitle.Contains(SearchFilter)) {
                        result.MoveToFront(item);
                    }
                    else if (item.Data.NormalTitle.Contains(SearchFilter)) {
                        result.MoveToFront(item);
                    }
                }

                foreach (var item in result) {
                    Resources.Add(item);
                    await Task.Delay(10);
                }

                IsLoading = false;
            }
            catch (Exception ex) {
                $"我去，炸了，详细信息如下：{ex.Message}".ShowMessage("错误");
            }
        }

        public async ValueTask SearchModrinthResourceAsync() {
            await ModrinthToolkit.SearchAsync(SearchFilter, ProjectType:CurrentCategorie.ToModrinthProjectType());
        }
        
        public void OpenGameInstallDialogAction() {
            MainWindow.Instance.Install.InstallDialog.ShowDialog();
        }

        public async void GetModrinthModpackAction() {
            IsLoading = true;
            await GetModrinthResourceAsync();
        }

        public async void GetCurseforgeModpackAction() {       
            IsLoading = true;
            await GetCurseforgeResourceAsync();
        }

        public async void SearchResourceAction() {
            IsLoading = true;
            await SearchCurseforgeResourceAsync();
        }

        public void OpenSearchOptionsAction() {
            SearcherHeight = 180;
        }

        public void CloseSearchOptionsAction() {       
            SearcherHeight = 0;
        }

        public void GoBackAction() {
            MainWindow.Instance.NavigationPage(new ActionCenterPage());
        }
    }
}
