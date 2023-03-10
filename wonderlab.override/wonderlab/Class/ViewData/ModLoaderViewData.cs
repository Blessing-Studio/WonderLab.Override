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
        }

        public string Type { get; set; }
    }
}

