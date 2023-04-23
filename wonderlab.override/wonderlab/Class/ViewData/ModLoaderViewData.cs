using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.Class.ViewData
{
    public class ModLoaderViewData : ViewDataBase<ModLoaderModel> { 
        public ModLoaderViewData(ModLoaderModel data) : base(data) {     
            Id = data.Id;
        }

        [Reactive]
        public string Type { get; set; }

        [Reactive]
        public string Id { get; set; }
    }
}

