using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.ViewModels.Pages
{
    public class WebConfigPageViewModel : ReactiveObject {
        public WebConfigPageViewModel() { 
            
        }

        [Reactive]
        public ObservableCollection<WebConnectionTestModel> TestList { get; set; } = new();

        public async void RunConnectionTestAction() {
            TestList.Clear();

            //下载源
            TestList.Add(new WebConnectionTestModel("https://bmclapi2.bangbang93.com"));
            TestList.Add(new WebConnectionTestModel("https://download.mcbbs.net"));
            TestList.Add(new WebConnectionTestModel("http://launchermeta.mojang.com"));

            //皮肤
            TestList.Add(new WebConnectionTestModel("https://sessionserver.mojang.com"));
            TestList.Add(new WebConnectionTestModel("https://www.minecraft.net"));

            //更新服务
            TestList.Add(new WebConnectionTestModel("http://8.218.142.204:8888/", "update.wonderapi.com"));
        }
    }
}
