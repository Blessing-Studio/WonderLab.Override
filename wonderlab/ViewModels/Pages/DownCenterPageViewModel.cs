using DynamicData;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class DownCenterPageViewModel : ViewModelBase {
        public IEnumerable<GameCoreEmtity> Cache;

        public DownCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            GetGameCoresAction();
        }

        private async void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentMcVersionType)) {
                GameCores = new(Cache.Where(x => x.Type.Contains(CurrentMcVersionType)));
            }
        }

        [Reactive]
        public ObservableCollection<WebModpackViewData> Resources { get; set; } = new();

        [Reactive]
        public ObservableCollection<GameCoreEmtity> GameCores { get; set; } = new();

        [Reactive]
        public bool IsLoading { get; set; }

        [Reactive]
        public bool IsResource { get; set; } = false;

        [Reactive]
        public bool IsLoaded { get; set; }

        [Reactive]
        public string SearchFilter { get; set; }

        [Reactive]
        public string CurrentMcVersion { get; set; } = string.Empty;

        [Reactive]
        public string CurrentMcVersionType { get; set; } = "正式版";

        [Reactive]
        public GameCoreEmtity CurrentGameCore { get; set; }

        [Reactive]
        public KeyValuePair<int, string> CurrentCategorie { get; set; } = new(6, "模组");

        [Reactive]
        public double SearcherHeight { get; set; } = 0;

        [Reactive]
        public ResourceType ResourceType { get; set; } = ResourceType.Minecraft;

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

        public List<string> McVersionTypes { get; } = new() {
            "正式版",
            "快照版",
            "远古版",
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
            try {
                Resources.Clear();

                var modpacks = await Task.Run(async () => await ModrinthUtil.GetFeaturedModpacksAsync());
                foreach (var i in modpacks.Hits.AsParallel()) {
                    await Task.Run(async () => {
                        var infos = await ModrinthUtil.GetProjectInfos(i.ProjectId);
                        Resources.Add(new WebModpackModel(i, infos).CreateViewData<WebModpackModel, WebModpackViewData>());
                    });
                }

                IsLoading = false;
            }
            catch (Exception ex) {
                if (ex.Message.Contains("Timeout of 30 seconds elapsing")) {
                    "WonderLab 在获取 Modrinth 资源时连接超时，请检查您的网络是否有问题！".ShowInfoDialog("拉取失败");
                } else {
                    $"WonderLab 在获取 Modrinth 资源时遭遇未知异常，信息如下：{ex}".ShowInfoDialog("程序遭遇异常");
                }
            }
        }

        public async ValueTask GetCurseforgeResourceAsync() {
            try {
                Resources.Clear();

                var modpacks = await Task.Run(async () => await GlobalResources.CurseForgeToolkit.GetFeaturedsAsync());
                foreach (var x in modpacks) {
                    Resources.Add(new WebModpackModel(x).CreateViewData<WebModpackModel, WebModpackViewData>());
                    await Task.Delay(10);
                }

                IsLoading = false;
            }
            catch (Exception ex) {
                if (ex.Message.Contains("Timeout of 30 seconds elapsing")) {
                    "WonderLab 在获取 Curseforge 资源时连接超时，请检查您的网络是否有问题！".ShowInfoDialog("拉取失败");
                } else {
                    $"WonderLab 在获取 Curseforge 资源时遭遇未知异常，信息如下：{ex}".ShowInfoDialog("程序遭遇异常");
                }
            }
        }

        public async ValueTask SearchCurseforgeResourceAsync() {
            try {
                //模组中文搜索检测
                var searchFilter = string.Empty;
                if (CurrentCategorie.Key == 6 && !string.IsNullOrEmpty(SearchFilter)) {
                    foreach (var item in CacheResources.WebModpackInfoDatas.AsParallel()) {
                        if (SearchFilter.IsChinese() && item.Value.Chinese.Contains(SearchFilter) && item.Value.Chinese.Contains("(") && item.Value.Chinese.Contains(")")) {
                            item.Value.CurseForgeId = item.Value.Chinese.Split(" (")[1].Split(")").First().Trim();
                            searchFilter = item.Value.CurseForgeId;

                            Trace.WriteLine($"[信息] 新的 CurseForgeId 值为 {searchFilter}");
                            break;
                        } else if (SearchFilter.IsChinese()) searchFilter = item.Value.CurseForgeId.Replace("-", " ");
                    }
                } else if (!string.IsNullOrEmpty(SearchFilter)) {
                    searchFilter = SearchFilter;
                }


                Resources.Clear();
                var result = (await GlobalResources.CurseForgeToolkit.SearchResourceAsync(string.IsNullOrEmpty(searchFilter) ? SearchFilter : searchFilter,
                    CurrentCategorie.Key, gameVersion: CurrentMcVersion.Contains("All") ? string.Empty : CurrentMcVersion))
                    .Select(x => new WebModpackModel(x).CreateViewData<WebModpackModel, WebModpackViewData>()).ToList();

                //重新排序
                var list = result.ToList();
                foreach (var item in list) {
                    if (!string.IsNullOrEmpty(SearchFilter) && SearchFilter.IsChinese() && item.Data.ChineseTitle.Contains(SearchFilter)) {
                        result.MoveToFront(item);
                    } else if (!string.IsNullOrEmpty(SearchFilter) && item.Data.NormalTitle.Contains(SearchFilter)) {
                        result.MoveToFront(item);
                    }
                }
                IsLoading = false;

                foreach (var item in result) {
                    Resources.Add(item);
                    await Task.Delay(10);
                }
            }
            catch (Exception ex) {
                $"我去，炸了，详细信息如下：{ex.Message}".ShowMessage("错误");
            }
        }

        public async ValueTask SearchModrinthResourceAsync() {
            Resources.Clear();

            var modpacks = await Task.Run(async () => await ModrinthUtil.SearchAsync(SearchFilter, ProjectType: CurrentCategorie.ToModrinthProjectType()));
            IsLoading = false;

            foreach (var i in modpacks.Hits.AsParallel()) {
                await Task.Run(async () => {
                    var infos = await ModrinthUtil.GetProjectInfos(i.ProjectId);
                    Resources.Add(new WebModpackModel(i, infos).CreateViewData<WebModpackModel, WebModpackViewData>());
                });
            }
        }

        public async ValueTask SearchGameCoreAsync() {
            GameCores.Clear();

            var result = Cache.Where(x => x.Id.Contains(SearchFilter)).ToList();
            IsLoading = IsLoaded = false;

            var filtered = result.Where(x => x.Type.Contains(CurrentMcVersionType));
            GameCores = new(filtered.ToList());
            IsLoaded = true;
        }

        public async void OpenGameInstallDialogAction() {
            "下载对话框开启，开始加载加载器信息".ShowLog();
            await Task.Run(async () => await HttpUtils.GetModLoadersFromMcVersionAsync(CacheResources.GameCoreInstallInfo.Id));
        }

        public async void GetGameCoresAction() {
            try {
                var result = await Task.Run(async () => await HttpUtils.GetGameCoresAsync());
                GameCores?.Clear();
                Cache = result.ToList();

                GameCores = new(Cache.Where(x => x.Type
                   .Contains(CurrentMcVersionType)));
            }
            catch (Exception ex) {
                $"网络异常，{ex.Message}".ShowMessage("错误");
            }
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

            switch (ResourceType) {
                case ResourceType.Minecraft:
                    await SearchGameCoreAsync();
                    break;
                case ResourceType.Curseforge:
                    await SearchCurseforgeResourceAsync();
                    break;
                case ResourceType.Modrinth:
                    await SearchModrinthResourceAsync();
                    break;
            }
        }

        public void OpenSearchOptionsAction() {
            SearcherHeight = 180;
        }

        public void CloseSearchOptionsAction() {
            SearcherHeight = 0;
        }

        public override void GoBackAction() {
            new ActionCenterPage().Navigation();
        }
    }
}
