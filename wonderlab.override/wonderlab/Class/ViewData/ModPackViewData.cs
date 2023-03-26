using MinecraftLaunch.Modules.Models.Download;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.ViewModels.Pages;

namespace wonderlab.Class.ViewData
{
    public class ModPackViewData : ViewDataBase<ModPack> {   
        public ModPackViewData(ModPack data) : base(data) {
        
        }

        public void ModPackStateCorrect() {
            var reslut = ModConfigPageViewModel.Toolkit.ModStateChange(Data.Path);
            Trace.WriteLine($"[信息] 更改后的模组的路径为 {reslut}");
        }
    }
}
