using DynamicData;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages {
    public class ServerFindPageViewModel : ViewModelBase {
        public ObservableCollection<WonderServerViewData> Servers { get; set; } = new();

        public async void GetServerListAction() {
            await GetServerListAsync();
        }

        public override void GoBackAction() {
            new ActionCenterPage().Navigation();
        }

        public async ValueTask GetServerListAsync() {
            try {
                Servers.Clear();

                var json = await HttpWrapper.HttpClient.GetStringAsync($"{GlobalResources.WonderApi}server");
                var viewDatas = json.ToJsonEntity<IEnumerable<WonderServerModel>>().Select(x => x.CreateViewData<WonderServerModel, WonderServerViewData>()).ToList();
                Servers.AddRange(viewDatas);

                foreach (var x in viewDatas.AsParallel()) {
                    try {
                        await x.GetServerInfoAction();
                        $"来自 {x.Data.Author} 的服务器延迟为 {x.ServerInfo.Latency}ms".ShowLog();
                    }
                    //Fix #18
                    catch (Exception ex) {
                        ex.ShowLog();
                    }
                }
            }
            catch (Exception ex) {
                ex.ShowLog();
            }
        }
    }
}
